using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialIntroduction : DialogueClasses
{
    [SerializeField] private Jackie jackie;
    [SerializeField] private Transform jackieDefaultTransform;
    [SerializeField] private EnemyIves ives;
    [SerializeField] private Transform ivesDefaultTransform;
    [SerializeField] private GameObject trainingDummyPrefab;
    [SerializeField] private Transform dummy1StartingPos;
    [SerializeField] private Transform jackieEndPosition;

    [SerializeField] private Transform ivesPassiveBattlePosition;
    [SerializeField] private List<GameObject> ivesTutorialDeck;

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
    //After Ives Starts fighting
    [SerializeField] private List<DialogueText> readingOpponentTutorial;
    [SerializeField] private List<DialogueText> clashingCardsTutorial;
    [SerializeField] private List<DialogueText> clashingOutcomeTutorial;
    [SerializeField] private List<DialogueText> cardsExhaustedTutorial;

    //After Ives is defeated
    [SerializeField] private List<DialogueText> ivesIsDefeated;
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
            //Text here handles dialogue before Jackie Ives combat
            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(openingDiscussion));

            jackie.Emphasize(); //Jackie shows up above the black background
            yield return StartCoroutine(jackie.ResetPosition()); //Jackie Runs into the scene and talks 
            yield return new WaitForSeconds(MEDIUM_PAUSE);

            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(jackieMonologue));
            yield return StartCoroutine(CombatManager.Instance.FadeInLightScreen(2f));
            jackie.DeEmphasize(); //Jackie is below the black background

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
            trainingDummies.Add(Instantiate(trainingDummyPrefab, dummy1StartingPos)); //Ives summons Dummy


            yield return new WaitForSeconds(1f);
            yield return StartCoroutine(jackie.MoveToPosition(dummy1StartingPos.position, 1.4f, 0.8f));
            yield return new WaitForSeconds(1f);
        } else
        {
            //Set up the scene for a combat Jump in.
            ives.SetReturnPosition(ivesDefaultTransform.position);
            StartCoroutine(ives.MoveToPosition(ivesDefaultTransform.position, 0, 0.1f)); //Ives comes into the scene
            StartCoroutine(ives.MoveToPosition(dummy1StartingPos.position, 1.2f, 0.1f)); //Ives goes to place a dummy down
            trainingDummies.Add(Instantiate(trainingDummyPrefab, dummy1StartingPos));
            yield return StartCoroutine(CombatManager.Instance.FadeInLightScreen(2f));
        }

        jackie.InCombat(); //Workaround for now, ill have to remove this once i manually start instantiating 
        ives.InCombat();
        ives.SetReturnPosition(ivesPassiveBattlePosition.position);
        StartCoroutine(ives.ResetPosition()); //Prevent Players from attacking Ives LOL
        ives.UnTargetable();

        DialogueManager.Instance.MoveBoxToTop();
        CombatManager.Instance.GameState = GameState.SELECTION;
        
        BeginCombatTutorial();
        yield return new WaitUntil(() => CombatManager.Instance.GameState == GameState.GAME_WIN);

        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(buffTutorial));
        
        //Ives retrieves the dead training dummy
        foreach (GameObject trainingDummy in trainingDummies)
        {
            yield return StartCoroutine(ives.MoveToPosition(trainingDummy.transform.position, 1.2f, 0.8f));
            yield return new WaitForSeconds(0.6f);
            Destroy(trainingDummy);
        }

        // Have Ives fight and teach clashing now.

        jackie.SetReturnPosition(jackie.gameObject.transform.position);
        ives.SetReturnPosition(dummy1StartingPos.position);
        ives.InjectDeck(ivesTutorialDeck);
        CombatManager.Instance.AddEnemy(ives);
        ives.Targetable();


        yield return new WaitUntil(() => !DialogueManager.Instance.IsInDialogue());
        CombatManager.Instance.GameState = GameState.SELECTION;

        BeginCombatIvesFight();

        yield return new WaitUntil(() => CombatManager.Instance.GameState == GameState.GAME_WIN);

        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(ivesIsDefeated));
        yield return new WaitForSeconds(0.6f);

        //Ives Stands back up
        ives.Heal(5);
        ives.SetUnstaggered();
        yield return new WaitForSeconds(0.8f);

        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(endingTutorialDialogue.Dialogue));


        StartCoroutine(CombatManager.Instance.FadeInDarkScreen(3f));
        yield return StartCoroutine(jackie.MoveToPosition(jackieEndPosition.position, 0, 4f));

        SceneManager.LoadScene("LevelSelect");

        yield break;
    }

    //Player first sees that they can play cards
    private void BeginCombatTutorial()
    {
        EntityClass.OnEntityDeath += FirstDummyDies; //Setup Listener to set state to Game Win
        PlayerClass.playerReshuffleDeck += PlayerLostOneMaxHandSize;
        StartCoroutine(StartDialogueWithNextEvent(youCanPlayCardsTutorial, () => { ActionClass.cardHighlightedEvent += OnPlayerFirstHighlightCard; }));
    }

    private void PlayerLostOneMaxHandSize(PlayerClass player)
    {
        if (player == jackie)
        {
            StartCoroutine(HandSizeDecreasedDialogue());
        }
    }

    private IEnumerator HandSizeDecreasedDialogue()
    {
        PlayerClass.playerReshuffleDeck -= PlayerLostOneMaxHandSize;
        yield return new WaitUntil(() => !DialogueManager.Instance.IsInDialogue());
        StartCoroutine(DialogueManager.Instance.StartDialogue(cardsExhaustedTutorial));
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
        if (entity is TrainingDummy)
        {
            EntityClass.OnEntityDeath -= FirstDummyDies;
            CombatManager.Instance.GameState = GameState.GAME_WIN;
        }
    }

    //-----------------------------------Ives Fight----------------------------------------

    private void BeginCombatIvesFight()
    {
        EntityClass.OnEntityDeath += IvesDies; //Setup Listener to set state to Game Win
        DialogueManager.Instance.MoveBoxToBottom();
        StartCoroutine(StartDialogueWithNextEvent(readingOpponentTutorial, () => { BattleQueue.playerActionInsertedEvent += OnPlayerPlayClashingCard; }));
    }

    private void OnPlayerPlayClashingCard(ActionClass actionClass)
    {
        BattleQueue.playerActionInsertedEvent -= OnPlayerPlayClashingCard;
        StartCoroutine(StartDialogueWithNextEvent(clashingCardsTutorial, () => { CardComparator.playersAreRollingDiceEvent += OnPlayerClashingWithIves; }));
    }

    private IEnumerator OnPlayerClashingWithIves()
    {
        CardComparator.playersAreRollingDiceEvent -= OnPlayerClashingWithIves;
        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(clashingOutcomeTutorial));
    }

    private void IvesDies(EntityClass entity)
    {
        if (entity is EnemyIves)
        {
            EntityClass.OnEntityDeath -= IvesDies;
            CombatManager.Instance.GameState = GameState.GAME_WIN;
        }
    }
//------------------------------------------------------Helpers---------------------------------------------------------------------------------

    private IEnumerator InstantiateDummies()
    {
        yield return StartCoroutine(ives.MoveToPosition(dummy1StartingPos.position, 1.2f, 0.8f)); //Ives goes to place a dummy down
        yield return new WaitForSeconds(0.3f);
        trainingDummies.Add(Instantiate(trainingDummyPrefab, dummy1StartingPos));
        yield return StartCoroutine(ives.MoveToPosition(jackieEndPosition.position, 1.2f, 0.8f)); //Ives goes to place a dummy down
        yield return new WaitForSeconds(0.3f);
        trainingDummies.Add(Instantiate(trainingDummyPrefab, jackieEndPosition));
    }

    //Helper to wait until dialogue is done, then start @param dialogue, then run a callback like setting up a new event. 
    private IEnumerator StartDialogueWithNextEvent(List<DialogueText> dialogue, Action callbackToRun)
    {
        yield return new WaitUntil(() => !DialogueManager.Instance.IsInDialogue());
        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(dialogue));
        callbackToRun();
    }

}
