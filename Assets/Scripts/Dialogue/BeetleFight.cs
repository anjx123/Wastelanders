using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.VisualScripting.Member;
using static UnityEngine.UI.Image;
using System.Reflection;
using Cinemachine;
//@author: Andrew
public class BeetleFight : DialogueClasses
{
    [SerializeField] private Jackie jackie;
    [SerializeField] private Transform jackieDefaultTransform;
    [SerializeField] private Transform jackieChasingTransform;
    [SerializeField] private Transform jackieTalkingTransform;
    [SerializeField] private Ives ives;
    [SerializeField] private Transform ivesDefaultTransform;

    [SerializeField] private WasteFrog frog;
    [SerializeField] private WasteFrog frogThatRunsAway;

    [SerializeField] private Beetle ambushBeetle;
    [SerializeField] private Transform ambushBeetleTransform;
    [SerializeField] private Beetle wrangledBeetle;
    [SerializeField] private Transform wrangledBeetleTransform;
    [SerializeField] private List<Beetle> campBeetles;
    [SerializeField] private List<Crystals> crystals;

    [SerializeField] private List<Transform> combatBeetleTransforms;

    [SerializeField] private ActionClass beetleAction;
    [SerializeField] private ActionClass jackieAction;

    [SerializeField] private GameObject background;
    [SerializeField] private CinemachineVirtualCamera sceneCamera;
    [SerializeField] private Sprite frogDeathSprite;
    [SerializeField] private Sprite jackieCrystalSprite;

    [SerializeField] private DialogueWrapper openingDiscussion;
    [SerializeField] private DialogueWrapper jackieSurprised;
    [SerializeField] private DialogueWrapper jackieChase;
    [SerializeField] private DialogueWrapper jackieBeetleCamp;
    [SerializeField] private DialogueWrapper narratorCamp;
    [SerializeField] private DialogueWrapper ivesConversation;
    [SerializeField] private DialogueWrapper twoPlayerCombatTutorial;
    [SerializeField] private DialogueWrapper ivesTutorial;
    [SerializeField] private DialogueWrapper postBattleDialogue;

    [SerializeField] private bool jumpToCombat;


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
        frog.OutOfCombat();
        frog.FaceLeft();
        frogThatRunsAway.OutOfCombat();
        frogThatRunsAway.FaceLeft();
        ambushBeetle.OutOfCombat();
        wrangledBeetle.OutOfCombat();
        jackie.SetReturnPosition(jackieDefaultTransform.position);
        sceneCamera.Priority = 2;
        if (!jumpToCombat)
        {
            //Lights Camera Action!!
            {
                Coroutine fade = StartCoroutine(CombatManager.Instance.FadeInLightScreen(2f));
                yield return new WaitForSeconds(1f);
                frogThatRunsAway.FaceRight();
                yield return fade;
            }

            //Jackie runs in gun blazing
            {
                Coroutine jackieRunCoroutine = StartCoroutine(jackie.MoveToPosition(jackieDefaultTransform.position, 0, 1f)); //Jackie Runs into the scene
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(MakeFrogJump(frogThatRunsAway, 1f));
                StartCoroutine(KillFrog(frogThatRunsAway)); //Just moves them offscreen and kills them
                frog.FaceRight();
                yield return jackieRunCoroutine;
            }

            //Jackie takes down a wild frog
            {
                jackie.AttackAnimation("IsShooting");
                yield return StartCoroutine(frog.StaggerEntities(jackie, frog, 0.2f));
                CombatManager.Instance.ActivateDynamicCamera();
                sceneCamera.Priority = 0;
                Coroutine frogTriesToRun = StartCoroutine(frog.MoveToPosition(frog.transform.position + new Vector3(-2, 0, 0), 0, 1f));
                yield return new WaitForSeconds(BRIEF_PAUSE);
                yield return StartCoroutine(jackie.MoveToPosition(frog.transform.position, 1f, 1f));
                StopCoroutine(frogTriesToRun);
                jackie.AttackAnimation("IsStaffing");
                yield return StartCoroutine(frog.StaggerEntities(jackie, frog, 0.3f));
                DieInScene(frog);
                //yield return StartCoroutine(jackie.MoveToPosition(frog.transform.position, 0f, 1f));
            }


            // BEETLE AMBUSH!!
            {
                void ShowCrystal()
                {
                    jackie.animator.enabled = false;
                    jackie.GetComponent<SpriteRenderer>().sprite = jackieCrystalSprite;
                    DialogueBox.DialogueBoxEvent -= ShowCrystal;
                }
                DialogueBox.DialogueBoxEvent += ShowCrystal;
                yield return StartCoroutine(DialogueManager.Instance.StartDialogue(openingDiscussion.Dialogue));

                beetleAction.Target = jackie;
                beetleAction.Origin = ambushBeetle;
                jackieAction.Target = ambushBeetle;
                jackieAction.Origin = jackie;

                //Using reflection to set speed because speed because its a protected setter
                PropertyInfo cardPropInfo = typeof(ActionClass).GetProperty(nameof(jackieAction.Speed), BindingFlags.Public | BindingFlags.Instance);
                MethodInfo setMethod = cardPropInfo.GetSetMethod(true);
                setMethod.Invoke(jackieAction, new object[] { /* jackieAction.Speed = */ 1 });

                yield return StartCoroutine(ambushBeetle.MoveToPosition(ambushBeetleTransform.position, 0, 0.4f));
                yield return StartCoroutine(DialogueManager.Instance.StartDialogue(jackieSurprised.Dialogue));
                jackie.animator.enabled = true;
                yield return StartCoroutine(CardComparator.Instance.ClashCards(jackieAction, beetleAction));
            }

            //Jackie Chases the beetle and we fade
            {
                sceneCamera.transform.position = CombatManager.Instance.dynamicCamera.transform.position;
                yield return StartCoroutine(ambushBeetle.MoveToPosition(jackieChasingTransform.position, 0, 1.2f)); // beetle runs away
                sceneCamera.Priority = 2;
                CombatManager.Instance.ActivateBaseCamera();
                yield return StartCoroutine(DialogueManager.Instance.StartDialogue(jackieChase.Dialogue));
                Coroutine fade = StartCoroutine(CombatManager.Instance.FadeInDarkScreen(1f));
                yield return StartCoroutine(jackie.MoveToPosition(jackieChasingTransform.position, 1f, 1f)); //Jackie Runs into the scene
                yield return fade;
            }

            yield return new WaitForSeconds(MEDIUM_PAUSE);

            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(jackieBeetleCamp.Dialogue));
            yield return StartCoroutine(CombatManager.Instance.FadeInLightScreen(1.2f));
            yield return new WaitForSeconds(BRIEF_PAUSE);

            StartCoroutine(jackie.MoveToPosition(jackieTalkingTransform.position, 0, 1.6f));   
            ShiftScene(-17, 2);
            yield return StartCoroutine(CombatManager.Instance.FadeInDarkScreen(1.5f));

            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(narratorCamp.Dialogue));

            ives.transform.position = ivesDefaultTransform.position;
            wrangledBeetle.transform.position = wrangledBeetleTransform.position;
            yield return StartCoroutine(CombatManager.Instance.FadeInLightScreen(2f));

            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(ivesConversation.Dialogue));
            yield return StartCoroutine(CombatManager.Instance.FadeInDarkScreen(1.5f));
            wrangledBeetle.transform.position = new Vector3(200, 200, 1);
            jackie.transform.position = jackieDefaultTransform.position;
            jackie.FaceRight();
            yield return StartCoroutine(CombatManager.Instance.FadeInLightScreen(2f));

        }
        else // setup scene
        {
            jackie.transform.position = jackieDefaultTransform.position;
            ives.transform.position = ivesDefaultTransform.position;
            RemoveEnemyFromScene(frog);
            RemoveEnemyFromScene(ambushBeetle);
            yield return StartCoroutine(CombatManager.Instance.FadeInLightScreen(2f));
        }

        // combat time!
        beetles_alive = campBeetles.Count;
        RemoveEnemyFromScene(wrangledBeetle);
        foreach (Crystals c in crystals)
        {
            RemoveEnemyFromScene(c);
        }
        for (int i = 0; i < campBeetles.Count; i++)
        {
            campBeetles[i].SetReturnPosition(combatBeetleTransforms[i].position);
            campBeetles[i].InCombat();
        }
        jackie.InCombat(); 
        ives.InCombat();
        ives.SetReturnPosition(ivesDefaultTransform.position);
        DialogueManager.Instance.MoveBoxToTop();
        CombatManager.Instance.GameState = GameState.SELECTION;
        HighlightManager.Instance.SetActivePlayer(jackie);
        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(twoPlayerCombatTutorial.Dialogue));
        Begin2PCombatTutorial();
        yield return new WaitUntil(() => CombatManager.Instance.GameState == GameState.GAME_WIN);

        DialogueManager.Instance.MoveBoxToBottom();
        CombatManager.Instance.GameState = GameState.OUT_OF_COMBAT;
        yield return new WaitForSeconds(2);
        jackie.OutOfCombat();
        ives.OutOfCombat();
        jackie.FaceLeft();
        ives.FaceRight();
        StartCoroutine(ives.MoveToPosition(ivesDefaultTransform.position, 0, 1.5f));
        yield return StartCoroutine(jackie.MoveToPosition(jackieTalkingTransform.position, 0, 1.5f));
        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(postBattleDialogue.Dialogue));
        yield return StartCoroutine(CombatManager.Instance.FadeInDarkScreen(1.5f));
    }

    private void Begin2PCombatTutorial()
    {
        HighlightManager.EntityClicked += EntityClicked;
        EntityClass.OnEntityDeath += EntityDied;
    }

    private void EntityClicked(EntityClass e)
    {
        if (e.GetType() == typeof(Ives))
        {
            StartCoroutine(TwoPlayerDialogue());
        }
    }

    private IEnumerator TwoPlayerDialogue()
    {
        HighlightManager.EntityClicked -= EntityClicked;
        yield return new WaitUntil(() => !DialogueManager.Instance.IsInDialogue());
        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(ivesTutorial.Dialogue));
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

    // "clashes" both entities, without rolling dice. e1 gets staggered modestly.
    // this is stolen from card comparator (code duplication!) if there is a better way of doing this pls lmk
    private IEnumerator NoCombatClash(EntityClass e1, EntityClass e2)
    {
        EntityClass origin = e1;
        EntityClass target = e2;
        float originRatio = 0.1f;
        float targetRatio = 1f - originRatio;
        Vector3 centeredDistance = (origin.myTransform.position * originRatio + targetRatio * target.myTransform.position);
        float bufferedRadius = 0.25f;
        float duration = 0.6f;

        float xBuffer = CardComparator.X_BUFFER;

        StartCoroutine(origin?.MoveToPosition(HorizontalProjector(centeredDistance, origin.myTransform.position, xBuffer), bufferedRadius, duration, centeredDistance));
        yield return StartCoroutine(target?.MoveToPosition(HorizontalProjector(centeredDistance, target.myTransform.position, xBuffer), bufferedRadius, duration, centeredDistance));
        e1.TakeDamage(e2, 5);
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
    IEnumerator MakeFrogJump(EntityClass origin, float jumpHeight)
    {
        yield return new WaitForSeconds(0.10f);
        if (origin.HasAnimationParameter("IsStaggered"))
        {
            origin.animator.SetBool("IsStaggered", true);
        }
        yield return new WaitForSeconds(0.16f);
        Vector3 originalPosition = origin.myTransform.position;
        origin.myTransform.position = originalPosition + new Vector3(0, jumpHeight, 0);
        yield return new WaitForSeconds(0.28f);
        origin.myTransform.position = originalPosition;
        yield return new WaitForSeconds(0.20f);
        if (origin.HasAnimationParameter("IsStaggered"))
        {
            origin.animator.SetBool("IsStaggered", false);
        }
    }

    IEnumerator KillFrog(WasteFrog wasteFrog)
    {
        int runDistance = -10;
        CombatManager.Instance.RemoveEnemy(wasteFrog);
        yield return StartCoroutine(wasteFrog.MoveToPosition(wasteFrog.transform.position + new Vector3(runDistance, 0, 0), 0, 1.2f));
        wasteFrog.gameObject.SetActive(false);
    }
    void DieInScene(WasteFrog wasteFrog)
    {
        BattleQueue.BattleQueueInstance.RemoveAllInstancesOfEntity(wasteFrog);
        CombatManager.Instance.RemoveEnemy(wasteFrog);
        wasteFrog.animator.enabled = false;
        wasteFrog.GetComponent<SpriteRenderer>().sprite = frogDeathSprite;
        wasteFrog.OutOfCombat();
        wasteFrog.UnTargetable();
        wasteFrog.combatInfo.gameObject.SetActive(false);
        wasteFrog.transform.rotation = Quaternion.Euler(0, 0, 75);
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
}
