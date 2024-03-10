using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class TutorialIntroduction : DialogueClasses
{
    [SerializeField] private Jackie jackie;
    [SerializeField] private Transform jackieDefaultTransform;
    [SerializeField] private EnemyIves ives;
    [SerializeField] private Transform ivesDefaultTransform;
    [SerializeField] private GameObject trainingDummyPrefab;
    [SerializeField] private Transform dummy1StartingPos;
    [SerializeField] private Transform dummy2StartingPos;
    [SerializeField] private Transform dummy3StartingPos;
    [SerializeField] private Transform ivesPassiveBattlePosition;

    [SerializeField] private List<DialogueText> openingDiscussion;
    [SerializeField] private List<DialogueText> jackieMonologue;
    [SerializeField] private List<DialogueText> soldierGreeting;
    [SerializeField] private List<DialogueText> jackieTalksWithSolider;
    [SerializeField] private DialogueWrapper ivesChatsWithJackie;

    //SingleDummyTutorial
    [SerializeField] private List<DialogueText> youCanPlayCardsTutorial;
    [SerializeField] private List<DialogueText> cardFieldsTutorial;
    [SerializeField] private List<DialogueText> queueUpActionsTutorial;
    [SerializeField] private List<DialogueText> rollingDiceTutorial;
    //Plays after first Dummy killed
    [SerializeField] private List<DialogueText> buffTutorial;
    //After three dummies are killed
    [SerializeField] private List<DialogueText> readingOpponentTutorial;
    [SerializeField] private List<DialogueText> clashingCardsTutorial;
    [SerializeField] private List<DialogueText> clashingOutcomeTutorial;
    [SerializeField] private List<DialogueText> cardsExhaustedTutorial;

    //After Ives is defeated
    [SerializeField] private DialogueWrapper endingTutorialDialogue;


    [SerializeField] private bool jumpToCombat;

    private List<GameObject> trainingDummies = new();



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
        jackie.SetReturnPosition(jackieDefaultTransform.position);
        if (!jumpToCombat)
        {
            yield return new WaitForSeconds(2f);

            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(openingDiscussion));

            jackie.Emphasize(); //Jackie shows up above the black background
            yield return StartCoroutine(jackie.ResetPosition()); //Jackie Runs into the scene and talks 
            yield return new WaitForSeconds(MEDIUM_PAUSE);

            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(jackieMonologue));
            yield return StartCoroutine(CombatManager.Instance.SetLightScreen());
            jackie.DeEmphasize();

            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(soldierGreeting));

            yield return new WaitForSeconds(1f);
            jackie.FaceLeft(); //Jackie faces the soldier talking to her
            yield return new WaitForSeconds(0.2f);

            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(jackieTalksWithSolider));

            yield return new WaitForSeconds(MEDIUM_PAUSE);

            ives.SetReturnPosition(ivesDefaultTransform.position);
            yield return StartCoroutine(ives.MoveToPosition(ivesDefaultTransform.position, 0, 0.8f)); //Ives comes into the scene

            yield return new WaitForSeconds(0.2f);
            jackie.FaceRight(); //Jackie turns to face the person approaching her

            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(ivesChatsWithJackie.Dialogue));

            yield return StartCoroutine(ives.MoveToPosition(dummy1StartingPos.position, 1.2f, 1.2f)); //Ives goes to place a dummy down
            trainingDummies.Add(Instantiate(trainingDummyPrefab, dummy1StartingPos));


            yield return new WaitForSeconds(1f);
            yield return StartCoroutine(jackie.MoveToPosition(dummy1StartingPos.position, 1.4f, 0.8f));


            yield return new WaitForSeconds(1f);
        } else
        {
            ives.SetReturnPosition(ivesDefaultTransform.position);
            StartCoroutine(ives.MoveToPosition(ivesDefaultTransform.position, 0, 0.1f)); //Ives comes into the scene
            StartCoroutine(ives.MoveToPosition(dummy1StartingPos.position, 1.2f, 0.1f)); //Ives goes to place a dummy down
            trainingDummies.Add(Instantiate(trainingDummyPrefab, dummy1StartingPos));
            yield return StartCoroutine(CombatManager.Instance.SetLightScreen());
        }

        ives.InCombat();
        jackie.InCombat(); //Workaround for now, ill have to remove this once i manually start instantiating players
        DialogueManager.Instance.MoveBoxToTop();
        ives.SetReturnPosition(ivesPassiveBattlePosition.position);
        CombatManager.Instance.GameState = GameState.SELECTION;
        BeginCombatTutorial();
         EntityClass.onEntityDeath += FirstDummyDies;
        yield return new WaitUntil(() => CombatManager.Instance.GameState == GameState.GAME_WIN);

        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(buffTutorial));

        foreach (GameObject trainingDummy in trainingDummies)
        {
            yield return StartCoroutine(ives.MoveToPosition(trainingDummy.transform.position, 1.2f, 0.8f));
            yield return new WaitForSeconds(0.6f);
            Destroy(trainingDummy);
        }
        // Have Ives fight and teach clashing now.
        ives.SetReturnPosition(dummy1StartingPos.position);
        jackie.SetReturnPosition(jackie.gameObject.transform.position);
        CombatManager.Instance.GameState = GameState.SELECTION;


        yield return new WaitUntil(() => CombatManager.Instance.GameState == GameState.GAME_WIN);

        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(endingTutorialDialogue.Dialogue));








        yield break;
    }

    //Player first sees that they can play cards
    private void BeginCombatTutorial()
    {
        StartCoroutine(StartDialogueWithNextEvent(youCanPlayCardsTutorial, () => { ActionClass.cardHighlightedEvent += OnPlayerFirstHighlightCard; }));
    }

    //Once hovering over a card, we talk about speed and power
    private void OnPlayerFirstHighlightCard(ActionClass card)
    {
        ActionClass.cardHighlightedEvent -= OnPlayerFirstHighlightCard;
        StartCoroutine(StartDialogueWithNextEvent(cardFieldsTutorial, () => { BattleQueue.playerActionInsertedEvent += OnPlayerFirstInsertCard; }));
    }

    //Once a player targets an enemy, we talk about the queue
    private void OnPlayerFirstInsertCard(ActionClass card)
    {
        DialogueManager.Instance.MoveBoxToBottom();
        BattleQueue.playerActionInsertedEvent -= OnPlayerFirstInsertCard;
        StartCoroutine(StartDialogueWithNextEvent(queueUpActionsTutorial, () => { CardComparator.playersAreRollingDiceEvent += OnPlayerFightsDummy; }));
    }

    private IEnumerator OnPlayerFightsDummy()
    {
        CardComparator.playersAreRollingDiceEvent -= OnPlayerFightsDummy;
        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(rollingDiceTutorial));
    }

    private void FirstDummyDies(EntityClass entity)
    {
        DialogueManager.Instance.MoveBoxToTop();
        if (entity is TrainingDummy)
        {
            EntityClass.onEntityDeath -= FirstDummyDies;
            CombatManager.Instance.GameState = GameState.GAME_WIN;
        }
    }









    private IEnumerator InstantiateDummies()
    {
        yield return StartCoroutine(ives.MoveToPosition(dummy1StartingPos.position, 1.2f, 0.8f)); //Ives goes to place a dummy down
        yield return new WaitForSeconds(0.3f);
        trainingDummies.Add(Instantiate(trainingDummyPrefab, dummy1StartingPos));
        yield return StartCoroutine(ives.MoveToPosition(dummy2StartingPos.position, 1.2f, 0.8f)); //Ives goes to place a dummy down
        yield return new WaitForSeconds(0.3f);
        trainingDummies.Add(Instantiate(trainingDummyPrefab, dummy2StartingPos));
        yield return StartCoroutine(ives.MoveToPosition(dummy3StartingPos.position, 1.2f, 0.8f)); //Ives goes to place a dummy down
        yield return new WaitForSeconds(0.3f);
        trainingDummies.Add(Instantiate(trainingDummyPrefab, dummy3StartingPos));
        yield return new WaitForSeconds(0.3f);
    }

    //Helper to start @param dialogue, then run a callback like setting up a new event. 
    private IEnumerator StartDialogueWithNextEvent(List<DialogueText> dialogue, Action callbackToRun)
    {
        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(dialogue));
        callbackToRun();
    }

}
