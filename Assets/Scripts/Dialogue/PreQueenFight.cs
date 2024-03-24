using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.VisualScripting.Member;

//@author: Andrew
public class PreQueenFight : DialogueClasses
{
    [SerializeField] private ScreenShake mainCamera;

    [SerializeField] private Jackie jackie;
    [SerializeField] private Sprite jackieStaff;
    [SerializeField] private Transform jackieDefaultTransform;
    [SerializeField] private Transform jackieTalkingTransform;
    [SerializeField] private Ives ives;
    [SerializeField] private Transform ivesDefaultTransform;

    [SerializeField] private Beetle draggerBeetle;
    [SerializeField] private Transform draggerBeetleTransform;
    [SerializeField] private Crystals draggedCrystal;
    [SerializeField] private List<Beetle> campBeetles;
    [SerializeField] private List<Crystals> crystals;
    [SerializeField] private Crystals bigCrystal;


    [SerializeField] private List<Transform> combatBeetleTransforms;

    [SerializeField] private GameObject background;

    [SerializeField] private bool jumpToCombat;

    [SerializeField] private DialogueWrapper IntroDialogue;
    [SerializeField] private DialogueWrapper MakingPlanDialogue;
    [SerializeField] private DialogueWrapper AfterBeetleFightDialogue;
    [SerializeField] private DialogueWrapper CrystalHitDialogue;
    [SerializeField] private DialogueWrapper PreQueenFightDialogue;

    private const float BRIEF_PAUSE = 0.2f; // For use after an animation to make it visually seem smoother
    private const float MEDIUM_PAUSE = 1f; //For use after a text box comes down and we want to add some weight to the text.

    private int beetles_alive;

    protected override void GameStateChange(GameState gameState)
    {
        if (gameState == GameState.GAME_START)
        {
            StartCoroutine(ExecuteGameStart());
        }
    }

    private IEnumerator ExecuteGameStart()
    {
        CombatManager.Instance.GameState = GameState.OUT_OF_COMBAT;
        CombatManager.Instance.SetDarkScreen();
        yield return new WaitForSeconds(1f);
        ives.OutOfCombat();
        jackie.OutOfCombat(); //Workaround for now, ill have to remove this once i manually start instantiating players
        draggerBeetle.OutOfCombat();
        jackie.SetReturnPosition(jackieDefaultTransform.position);
        foreach (Crystals c in crystals)
        {
            c.OutOfCombat();
        }
        foreach (Beetle b in campBeetles)
        {
            b.OutOfCombat();
            if (b.GetType() != typeof(WorkerBeetle))
            {
                b.FaceLeft();
            }
        }

        if (!jumpToCombat)
        {
            yield return new WaitForSeconds(1f);

            jackie.Emphasize(); //Jackie shows up above the black background

            yield return new WaitForSeconds(MEDIUM_PAUSE);

            yield return StartCoroutine(CombatManager.Instance.FadeInLightScreen(2f));

            ives.SetReturnPosition(ivesDefaultTransform.position);
            StartCoroutine(ives.ResetPosition());
            yield return StartCoroutine(jackie.ResetPosition()); //Jackie Runs into the scene
            yield return new WaitForSeconds(MEDIUM_PAUSE);
            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(IntroDialogue.Dialogue));

            yield return new WaitForSeconds(BRIEF_PAUSE);

            yield return StartCoroutine(BeetleDragCrystal(draggerBeetle, draggedCrystal, draggerBeetleTransform.position, 2f));

            yield return new WaitForSeconds(MEDIUM_PAUSE);

            yield return StartCoroutine(draggerBeetle.Die());

            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(MakingPlanDialogue.Dialogue));

            StartCoroutine(NoCombatClash(jackie, campBeetles[3], false));
            yield return new WaitForSeconds(BRIEF_PAUSE);
            yield return StartCoroutine(NoCombatClash(ives, campBeetles[0], false));
            yield return new WaitForSeconds(BRIEF_PAUSE);
            StartCoroutine(campBeetles[0].Die());
            StartCoroutine(campBeetles[3].Die());

            yield return new WaitForSeconds(MEDIUM_PAUSE);//TODO: layering

            StartCoroutine(NoCombatClash(jackie, campBeetles[2], false));
            yield return new WaitForSeconds(BRIEF_PAUSE);
            yield return StartCoroutine(NoCombatClash(ives, campBeetles[1], false));
            yield return new WaitForSeconds(BRIEF_PAUSE);
            StartCoroutine(campBeetles[1].Die());
            StartCoroutine(campBeetles[2].Die());

            yield return new WaitForSeconds(MEDIUM_PAUSE);
            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(AfterBeetleFightDialogue.Dialogue));

            yield return StartCoroutine(jackie.MoveToPosition(bigCrystal.transform.position, 1f, 0.5f));

            jackie.AttackAnimation("IsStaffing");
            yield return new WaitForSeconds(MEDIUM_PAUSE);

            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(CrystalHitDialogue.Dialogue));

            //TODO: shake?

            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(PreQueenFightDialogue.Dialogue));
        }
        else //setup scene
        {

        }
    }

    private void EntityDied(EntityClass e)
    {
        if (e.GetType().IsSubclassOf(typeof(Beetle)))
        {
            beetles_alive--;
            if (beetles_alive < 1)
            {
                CombatManager.Instance.GameState = GameState.GAME_WIN;
                EntityClass.OnEntityDeath -= EntityDied;
            }
        }
    }


    //------helpers------

    // "clashes" both entities, without rolling dice. bool parameter decides who gets staggered
    // this is stolen from card comparator (code duplication!) if there is a better way of doing this pls lmk
    private IEnumerator NoCombatClash(EntityClass e1, EntityClass e2, bool e1GetsHit)
    {
        EntityClass origin = e1;
        EntityClass target = e2;
        float originRatio = 0.5f;
        float targetRatio = 1f - originRatio;
        Vector3 centeredDistance = (origin.myTransform.position * originRatio + targetRatio * target.myTransform.position);
        float bufferedRadius = 0.25f;
        float duration = 0.6f;

        float xBuffer = CardComparator.X_BUFFER;

        StartCoroutine(origin?.MoveToPosition(HorizontalProjector(centeredDistance, origin.myTransform.position, xBuffer), bufferedRadius, duration, centeredDistance));
        yield return StartCoroutine(target?.MoveToPosition(HorizontalProjector(centeredDistance, target.myTransform.position, xBuffer), bufferedRadius, duration, centeredDistance));
        if (e1GetsHit)
        {
            e1.TakeDamage(e2, 100);
        }
        else
        {
            e2.TakeDamage(e1, 100);
        }
    }

    private Vector3 HorizontalProjector(Vector3 centeredDistance, Vector3 currentPosition, float xBuffer)
    {
        Vector3 vectorToCenter = (centeredDistance - currentPosition);

        return vectorToCenter.x > 0 ?
            currentPosition + vectorToCenter - new Vector3(xBuffer, 0f, 0f) :
            currentPosition + vectorToCenter + new Vector3(xBuffer, 0f, 0f);
    }

    // shifts the whole scene minus jackie to give the illusion of moving. i realize as i write this comment
    // that it would have been easier to just move jackie and the camera. but i am stupid. and i don't want to
    // redo this.
    private void ShiftScene(float shiftDistance, float shiftDuration)
    {
        StartCoroutine(ShiftObjectCoroutine(background, shiftDistance, shiftDuration)); // shift background

        // shift beetles; worker beetles stay facing right
        foreach (Beetle b in campBeetles)
        {
            b.OutOfCombat();
            if (b.GetType() != typeof(WorkerBeetle))
            {
                b.FaceLeft();
            }
            StartCoroutine(ShiftObjectCoroutine(b.gameObject, shiftDistance, shiftDuration));
        }

        //shift crystals
        foreach (Crystals c in crystals)
        {
            c.OutOfCombat();
            StartCoroutine(ShiftObjectCoroutine(c.gameObject, shiftDistance, shiftDuration));
        }
    }

    // moves the background. positive units shift to the left
    private IEnumerator ShiftObjectCoroutine(GameObject g, float shiftDistance, float shiftDuration)
    {
        float elapsedTime = 0.0f;
        Vector3 initialPosition = g.transform.position;
        while (elapsedTime < shiftDuration)
        {
            elapsedTime += Time.deltaTime;

            // Calculate the new position based on the elapsed time and shift duration
            float t = Mathf.Clamp01(elapsedTime / shiftDuration);
            Vector3 newPosition = initialPosition + shiftDistance * t * Vector3.left;

            // Update the position of the background
            g.transform.position = newPosition;

            yield return null; // Wait for the next frame
        }

    }

    // removes the entity from combat
    private void RemoveEnemyFromScene(EnemyClass e)
    {
        BattleQueue.BattleQueueInstance.RemoveAllInstancesOfEntity(e);
        CombatManager.Instance.RemoveEnemy(e);
        StartCoroutine(e.Die());
        e.gameObject.SetActive(false);
    }

    public IEnumerator MakeJump(GameObject obj, float jumpHeight, float jumpDuration)
    {
        // Set the start and end positions
        Vector3 startPosition = obj.transform.position;
        Vector3 endPosition = new(obj.transform.position.x, obj.transform.position.y + jumpHeight, obj.transform.position.z);

        // Start the jump coroutine
        yield return StartCoroutine(JumpCoroutine(obj, startPosition, endPosition, jumpDuration));
    }

    private IEnumerator JumpCoroutine(GameObject obj, Vector3 startPosition, Vector3 endPosition, float jumpDuration)
    {
        float elapsedTime = 0;

        // First half of the jump
        while (elapsedTime < jumpDuration / 2)
        {
            obj.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / (jumpDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Second half of the jump
        while (elapsedTime < jumpDuration)
        {
            obj.transform.position = Vector3.Lerp(endPosition, startPosition, (elapsedTime - jumpDuration / 2) / (jumpDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the object returns to the exact start position
        transform.position = startPosition;
    }

    // assumes the crystal is in the right spot lol to begin with
    private IEnumerator BeetleDragCrystal(Beetle b, Crystals c, Vector3 destination, float duration)
    {
        //bool goingLeft = b.transform.position.x > destination.x;
        //if (goingLeft)
        //{

        //}
        //else
        //{

        //}

        float distance = b.transform.position.x - c.transform.position.x;
        Vector3 crystalDestination = new(destination.x - distance, destination.y, destination.z);

        StartCoroutine(b.MoveToPosition(destination, 0, duration));
        yield return StartCoroutine(c.CrystalMoveToPosition(crystalDestination, 0, duration));
        yield return null;
    }
}
