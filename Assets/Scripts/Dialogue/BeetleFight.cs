using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.VisualScripting.Member;

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
    [SerializeField] private Beetle ambushBeetle;
    [SerializeField] private Transform ambushBeetleTransform;
    [SerializeField] private Beetle wrangledBeetle;
    [SerializeField] private Transform wrangledBeetleTransform;
    [SerializeField] private List<Beetle> campBeetles;
    [SerializeField] private List<Crystals> crystals;

    [SerializeField] private List<Transform> combatBeetleTransforms;

    [SerializeField] private GameObject background;

    [SerializeField] private DialogueWrapper openingDiscussion;
    [SerializeField] private DialogueWrapper jackieSurprised;
    [SerializeField] private DialogueWrapper jackieChase;
    [SerializeField] private DialogueWrapper jackieBeetleCamp;
    [SerializeField] private DialogueWrapper narratorCamp;
    [SerializeField] private DialogueWrapper ivesConversation;
    [SerializeField] private DialogueWrapper TwoPlayerCombatTutorial;
    [SerializeField] private DialogueWrapper ivesTutorial;

    [SerializeField] private bool jumpToCombat;


    private const float BRIEF_PAUSE = 0.2f; // For use after an animation to make it visually seem smoother
    private const float MEDIUM_PAUSE = 1f; //For use after a text box comes down and we want to add some weight to the text.

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
        ambushBeetle.OutOfCombat();
        wrangledBeetle.OutOfCombat();
        jackie.SetReturnPosition(jackieDefaultTransform.position);
        if (!jumpToCombat)
        {
            yield return new WaitForSeconds(1f);

            jackie.Emphasize(); //Jackie shows up above the black background
            
            yield return new WaitForSeconds(MEDIUM_PAUSE);

            yield return StartCoroutine(CombatManager.Instance.FadeInLightScreen(2f));

            yield return StartCoroutine(jackie.ResetPosition()); //Jackie Runs into the scene
            


            jackie.DeEmphasize(); //Jackie is below the black background
            yield return new WaitForSeconds(MEDIUM_PAUSE);
            frog.FaceLeft(); // frog sees jackie
            //TODO: make the frog jump
            yield return new WaitForSeconds(MEDIUM_PAUSE);
            yield return StartCoroutine(frog.Die()); // frog runs away
            yield return new WaitForSeconds(MEDIUM_PAUSE);
            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(openingDiscussion.Dialogue));

            // BEETLE AMBUSH!!

            ambushBeetle.FaceLeft();
            yield return StartCoroutine(ambushBeetle.MoveToPosition(ambushBeetleTransform.position, 0, 0.4f));
            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(jackieSurprised.Dialogue));
            yield return StartCoroutine(NoCombatClash(ambushBeetle, jackie));
            yield return new WaitForSeconds(MEDIUM_PAUSE);
            ambushBeetle.FaceRight();
            yield return StartCoroutine(ambushBeetle.Die()); // beetle runs away
            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(jackieChase.Dialogue));

            ShiftScene(17, 2);

            yield return StartCoroutine(jackie.MoveToPosition(jackieChasingTransform.position, 0, 2f)); //Jackie Runs into the scene
            yield return new WaitForSeconds(MEDIUM_PAUSE);

            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(jackieBeetleCamp.Dialogue));
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
            yield return StartCoroutine(CombatManager.Instance.FadeInLightScreen(2f));
        }

        // combat time!
        RemoveEnemyFromScene(wrangledBeetle);
        foreach (Crystals c in crystals)
        {
            RemoveEnemyFromScene(c);
        }
        for (int i = 0; i < campBeetles.Count; i++)
        {
            StartCoroutine(campBeetles[i].MoveToPosition(combatBeetleTransforms[i].position, 0, 1.5f));
            campBeetles[i].SetReturnPosition(combatBeetleTransforms[i].position);
        }
        jackie.InCombat(); 
        ives.InCombat();
        ives.SetReturnPosition(ivesDefaultTransform.position);
        CombatManager.Instance.AddPlayer(ives);
        DialogueManager.Instance.MoveBoxToTop();
        CombatManager.Instance.GameState = GameState.SELECTION;
        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(TwoPlayerCombatTutorial.Dialogue));
        Begin2PCombatTutorial();
        yield return new WaitUntil(() => CombatManager.Instance.GameState == GameState.GAME_WIN);
    }

    private void Begin2PCombatTutorial()
    {
        HighlightManager.EntityClicked += EntityClicked;
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
        StartCoroutine(DialogueManager.Instance.StartDialogue(ivesTutorial.Dialogue));
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
        e.gameObject.SetActive(false);
    }
}
