using LevelSelectInformation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using Systems.Persistence;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static BattleIntroEnum;

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



    [SerializeField] private Sprite laidBackSprite;
    [SerializeField] private Sprite puzzledSoldierSprite;
    [SerializeField] private Image laidBackImageUI;
    [SerializeField] private Image puzzeledImageUI;

    [SerializeField] private List<GameObject> ivesTutorialDeck;
    [SerializeField] private List<GameObject> jackieTutorialDeck;

    [SerializeField] private DialogueWrapper openingDiscussion;
    [SerializeField] private DialogueWrapper jackieMonologue;
    [SerializeField] private DialogueWrapper soldierGreeting;
    [SerializeField] private DialogueWrapper jackieTalksWithSolider;
    [SerializeField] private DialogueWrapper ivesChatsWithJackie;

    //SingleDummyTutorial
    [SerializeField] private DialogueWrapper youCanPlayCardsTutorial;
    [SerializeField] private DialogueWrapper cardFieldsTutorial;
    [SerializeField] private DialogueWrapper queueUpActionsTutorial;
    [SerializeField] private DialogueWrapper rollingDiceTutorial;
    //Plays after first Dummy killed
    [SerializeField] private DialogueWrapper buffTutorial;
    //After Ives Starts fighting
    [SerializeField] private DialogueWrapper readingOpponentTutorial;
    [SerializeField] private DialogueWrapper clashingCardsTutorial;
    [SerializeField] private DialogueWrapper defensiveCardsTutorial;
    [SerializeField] private DialogueWrapper cardAbilitiesTutorial;
    [SerializeField] private DialogueWrapper clashingOutcomeTutorial;
    [SerializeField] private DialogueWrapper clashingStrategyTutorial;
    [SerializeField] private DialogueWrapper cardsExhaustedTutorial;

    //After Ives is defeated
    [SerializeField] private DialogueWrapper ivesIsDefeated;
    [SerializeField] private DialogueWrapper endingTutorialDialogue;

    [SerializeField] private GameOver gameOver;
    [SerializeField] private DialogueWrapper gameLoseDialogue;
    [SerializeField] private BattleIntro battleIntro;

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

    public void OnDestroy()
    {
        CombatManager.ClearEvents();
        DialogueBox.ClearDialogueEvents();
        DialogueBox.DialogueBoxEvent -= SubscribeFirstHighlightMethod;
        PlayerClass.playerReshuffleDeck -= PlayerLostOneMaxHandSize;
        ActionClass.CardHighlightedEvent -= OnPlayerFirstHighlightCard;
        EntityClass.OnEntityDeath -= FirstDummyDies;
        DisplayableClass.OnShowCard -= ExplainDefense;
    }

    private IEnumerator ExecuteGameStart()
    {
        CombatManager.Instance.GameState = GameState.OUT_OF_COMBAT;
        CombatManager.Instance.SetDarkScreen();
        yield return new WaitForSeconds(1f);
        ives.OutOfCombat();
        jackie.OutOfCombat();
        jackie.SetReturnPosition(jackieDefaultTransform.position);
        battleIntro = BattleIntro.Build(Camera.main);
        if (!jumpToCombat && !GameStateManager.Instance.JumpToCombat)
        {
            yield return new WaitForSeconds(1f);

            { 
                int broadcastCounts = 0;
                void CountBroadcasts()
                {
                    broadcastCounts++;
                }
                void SetSpeaker(Image speaker, Image nonSpeaker)
                {
                    speaker.color = Color.white;
                    nonSpeaker.color = Color.gray;
                }


                DialogueBox.DialogueBoxEvent += CountBroadcasts;

                Coroutine dialogue = StartCoroutine(DialogueManager.Instance.StartDialogue(openingDiscussion.Dialogue));

                //Dialogue without any expression is kinda dry, its also difficult to tell whos talking so I wont VN style this unless I want to add more movement to the guys on screen

                yield return new WaitUntil(() => broadcastCounts == 1);
                StartCoroutine(FadeImage(laidBackImageUI, 1, true));
                yield return new WaitUntil(() => broadcastCounts == 2);
                StartCoroutine(FadeImage(puzzeledImageUI, 1, true)); //puzzled active
                laidBackImageUI.color = Color.gray;
                yield return new WaitUntil(() => broadcastCounts == 3);
                SetSpeaker(laidBackImageUI, puzzeledImageUI);
                yield return new WaitUntil(() => broadcastCounts == 4);
                SetSpeaker(puzzeledImageUI, laidBackImageUI);
                yield return new WaitUntil(() => broadcastCounts == 5);
                SetSpeaker(laidBackImageUI, puzzeledImageUI);
                yield return new WaitUntil(() => broadcastCounts == 6);
                SetSpeaker(puzzeledImageUI, laidBackImageUI);
                yield return new WaitUntil(() => broadcastCounts == 7);
                SetSpeaker(laidBackImageUI, puzzeledImageUI);
                yield return new WaitUntil(() => broadcastCounts == 8);
                SetSpeaker(puzzeledImageUI, laidBackImageUI);
                yield return dialogue;


                Coroutine laidBackFade = StartCoroutine(FadeImage(laidBackImageUI, 1, false));
                yield return StartCoroutine(FadeImage(puzzeledImageUI, 1, false));
                yield return laidBackFade;
                puzzeledImageUI.gameObject.SetActive(false);
                laidBackImageUI.gameObject.SetActive(false);

                DialogueBox.DialogueBoxEvent -= CountBroadcasts;
                yield return new WaitForSeconds(BRIEF_PAUSE);
            }

            jackie.Emphasize(); //Jackie shows up above the black background
            yield return StartCoroutine(jackie.MoveToPosition(jackieDefaultTransform.position, 0, 1.5f)); //Jackie Runs into the scene and talks 
            yield return new WaitForSeconds(MEDIUM_PAUSE);

            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(jackieMonologue.Dialogue));
            yield return StartCoroutine(CombatManager.Instance.FadeInLightScreen(2f));
            jackie.DeEmphasize(); //Jackie is below the black background

            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(soldierGreeting.Dialogue));
            yield return new WaitForSeconds(1f);

            jackie.FaceLeft(); //Jackie faces the soldier talking to her
            yield return new WaitForSeconds(0.2f);

            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(jackieTalksWithSolider.Dialogue));
            yield return new WaitForSeconds(MEDIUM_PAUSE);

            ives.SetReturnPosition(ivesDefaultTransform.position);
            yield return StartCoroutine(ives.MoveToPosition(ivesDefaultTransform.position, 0, 1f)); //Ives comes into the scene
            yield return new WaitForSeconds(0.2f);

            jackie.FaceRight(); //Jackie turns to face the person approaching her

            {
                DialogueBox.DialogueBoxEvent += JackieGoesToGetStaff;
                void JackieGoesToGetStaff()
                {
                    DialogueBox.DialogueBoxEvent -= JackieGoesToGetStaff;
                    StartCoroutine(jackie.MoveToPosition(dummy1StartingPos.position, 1.4f, 0.8f));
                }
                yield return StartCoroutine(DialogueManager.Instance.StartDialogue(ivesChatsWithJackie.Dialogue));
            }
            yield return StartCoroutine(ives.MoveToPosition(dummy1StartingPos.position, 1.2f, 0.8f)); //Ives goes to place a dummy down
            trainingDummies.Add(Instantiate(trainingDummyPrefab, dummy1StartingPos)); //Ives summons Dummy
        } else
        {
            GameStateManager.Instance.JumpToCombat = false;
            //Set up the scene for a combat Jump in.
            ives.SetReturnPosition(ivesDefaultTransform.position);
            StartCoroutine(CombatManager.Instance.FadeInLightScreen(2f));
            yield return StartCoroutine(ives.MoveToPosition(dummy1StartingPos.position, 1.2f, 0.8f)); //Ives goes to place a dummy down
            yield return new WaitForSeconds(BRIEF_PAUSE);
            trainingDummies.Add(Instantiate(trainingDummyPrefab, dummy1StartingPos));
            
        }

        yield return new WaitForSeconds(BRIEF_PAUSE);
        battleIntro.PlayAnimation(Get<TutorialIntro>());
        yield return new WaitForSeconds(1f);

        jackie.InjectDeck(jackieTutorialDeck);
        jackie.InCombat(); //Workaround for now, ill have to remove this once i manually start instantiating 
        ives.SetReturnPosition(ivesPassiveBattlePosition.position);
        StartCoroutine(ives.ResetPosition()); //Prevent Players from attacking Ives LOL
        CombatManager.Instance.SetEnemiesPassive(new List<EnemyClass> { ives });

        DialogueManager.Instance.MoveBoxToTop();
        CombatManager.Instance.BeginCombat();
        
        BeginCombatTutorial();
        yield return new WaitUntil(() => CombatManager.Instance.GameState == GameState.GAME_WIN);

        yield return new WaitUntil(() => !DialogueManager.Instance.IsInDialogue());
        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(buffTutorial.Dialogue));
        
        //Ives retrieves the dead training dummy
        foreach (GameObject trainingDummy in trainingDummies)
        {
            trainingDummy.GetComponent<SpriteRenderer>().sortingOrder -= 1;
            yield return StartCoroutine(ives.MoveToPosition(trainingDummy.transform.position, 1.2f, 0.8f));
            yield return new WaitForSeconds(0.6f);
            Destroy(trainingDummy);
            yield return new WaitForSeconds(1f);
        }

        // Have Ives fight and teach clashing now.

        ives.SetReturnPosition(dummy1StartingPos.position);
        ives.InjectDeck(ivesTutorialDeck);
        CombatManager.Instance.SetEnemiesHostile(new List<EnemyClass> { ives });

        yield return new WaitUntil(() => !DialogueManager.Instance.IsInDialogue());
        CombatManager.Instance.GameState = GameState.SELECTION;
        yield return new WaitUntil(() => !DialogueManager.Instance.IsInDialogue());
        BeginCombatIvesFight();

        yield return new WaitUntil(() => CombatManager.Instance.GameState == GameState.GAME_WIN);
        CombatManager.Instance.GameState = GameState.OUT_OF_COMBAT;
        ives.OutOfCombat();

        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(ivesIsDefeated.Dialogue));
        yield return new WaitForSeconds(0.6f);

        //Ives Stands back up
        ives.Heal(5);
        ives.SetUnstaggered();
        yield return new WaitForSeconds(0.8f);

        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(endingTutorialDialogue.Dialogue));

        AudioManager.Instance.FadeOutCurrentBackgroundTrack(2f);
        StartCoroutine(CombatManager.Instance.FadeInDarkScreen(3f));
        yield return StartCoroutine(jackie.MoveToPosition(jackieEndPosition.position, 0, 4f));

        GameStateManager.Instance.UpdateLevelProgress(StageInformation.DECK_SELECTION_TUTORIAL);
        GameStateManager.Instance.LoadScene(SceneData.Get<SceneData.SelectionScreen>().SceneName);
        yield break;
    }

    //Player first sees that they can play cards
    private void BeginCombatTutorial()
    {
        EntityClass.OnEntityDeath += FirstDummyDies; //Setup Listener to set state to Game Win
        PlayerClass.playerReshuffleDeck += PlayerLostOneMaxHandSize;
        StartCoroutine(StartTutorial());
    }

    private IEnumerator StartTutorial()
    {
        yield return new WaitUntil(() => !DialogueManager.Instance.IsInDialogue());
        DialogueBox.DialogueBoxEvent += SubscribeFirstHighlightMethod;
        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(youCanPlayCardsTutorial.Dialogue));   
    }

    void SubscribeFirstHighlightMethod()
    {
        DialogueBox.DialogueBoxEvent -= SubscribeFirstHighlightMethod;
        ActionClass.CardHighlightedEvent += OnPlayerFirstHighlightCard;
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
        DialogueManager.Instance.MoveBoxToTop();
        StartCoroutine(DialogueManager.Instance.StartDialogue(cardsExhaustedTutorial.Dialogue));
    }
    //Once hovering over a card, we talk about speed and power
    private void OnPlayerFirstHighlightCard(ActionClass card)
    {
        ActionClass.CardHighlightedEvent -= OnPlayerFirstHighlightCard;
        DialogueManager.Instance.DisplayNextSentence();
        StartCoroutine(StartDialogueWithNextEvent(cardFieldsTutorial.Dialogue, () => { HighlightManager.Instance.PlayerManuallyInsertedAction += OnPlayerFirstInsertCard; }));
    }

    //Once a player targets an enemy, we talk about the queue
    private void OnPlayerFirstInsertCard(ActionClass card)
    {
        DialogueManager.Instance.MoveBoxToBottom();
        HighlightManager.Instance.PlayerManuallyInsertedAction -= OnPlayerFirstInsertCard;
        StartCoroutine(StartDialogueWithNextEvent(queueUpActionsTutorial.Dialogue, () => { CardComparator.Instance.playersAreRollingDiceEvent += OnPlayerFightsDummy; }));
    }

    private IEnumerator OnPlayerFightsDummy()
    {
        CardComparator.Instance.playersAreRollingDiceEvent -= OnPlayerFightsDummy;
        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(rollingDiceTutorial.Dialogue));
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
        CombatManager.PlayersWinEvent += IvesDies; //Setup Listener to set state to Game Win
        CombatManager.EnemiesWinEvent += EnemiesWin;
        CombatManager.OnGameStateChanged += ExplainAbilities;
        StartCoroutine(StartDialogueWithNextEvent(readingOpponentTutorial.Dialogue, () => { HighlightManager.Instance.PlayerManuallyInsertedAction += OnPlayerPlayClashingCard; }));
    }

    private void OnPlayerPlayClashingCard(ActionClass actionClass)
    {
        HighlightManager.Instance.PlayerManuallyInsertedAction -= OnPlayerPlayClashingCard;
        StartCoroutine(StartDialogueWithNextEvent(clashingCardsTutorial.Dialogue, () => { CardComparator.Instance.playersAreRollingDiceEvent += OnPlayerClashingWithIves; }));
    }

    private IEnumerator OnPlayerClashingWithIves()
    {
        CardComparator.Instance.playersAreRollingDiceEvent -= OnPlayerClashingWithIves;
        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(clashingOutcomeTutorial.Dialogue));
    }

    private void ExplainAbilities(GameState gameState)
    {
        if (gameState == GameState.SELECTION)
        {
            CombatManager.OnGameStateChanged -= ExplainAbilities;
            StartCoroutine(StartDialogueWithNextEvent(cardAbilitiesTutorial.Dialogue, () => { DisplayableClass.OnShowCard += ExplainDefense; CombatManager.OnGameStateChanged += GiveClashAdvice; }));
        }

    }

    private void ExplainDefense(ActionClass actionClass)
    {
        DisplayableClass.OnShowCard -= ExplainDefense;
        if (actionClass is Brace) StartCoroutine(StartDialogueWithNextEvent(defensiveCardsTutorial.Dialogue, () => { }));
    }

    private void GiveClashAdvice(GameState gameState)
    {
        if (gameState == GameState.SELECTION)
        {
            CombatManager.OnGameStateChanged -= GiveClashAdvice;
            StartCoroutine(StartDialogueWithNextEvent(clashingStrategyTutorial.Dialogue, () => { }));
        }
    }
    private void IvesDies()
    {
        CombatManager.PlayersWinEvent -= IvesDies; //Setup Listener to set state to Game Win
        CombatManager.EnemiesWinEvent -= EnemiesWin;
        CombatManager.Instance.GameState = GameState.GAME_WIN;
    }

    private void EnemiesWin()
    {
        StartCoroutine(GameLose());
        CombatManager.PlayersWinEvent -= IvesDies;
        CombatManager.EnemiesWinEvent -= EnemiesWin;
        CombatManager.Instance.GameState = GameState.GAME_LOSE;
    }

    private IEnumerator GameLose()
    {
        yield return StartCoroutine(CombatManager.Instance.FadeInDarkScreen(2f));

        gameOver.gameObject.SetActive(true);
        gameOver.FadeIn();

        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(gameLoseDialogue.Dialogue));
    }
    //------------------------------------------------------Helpers---------------------------------------------------------------------------------

    IEnumerator FadeImage(Image image, float duration, bool fadeIn)
    {
        if (fadeIn)
        {
            // Fade in
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
            while (image.color.a < 1.0f)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a + (Time.deltaTime / duration));
                yield return null;
            }
        }
        else
        {
            // Fade out
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
            while (image.color.a > 0.0f)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a - (Time.deltaTime / duration));
                yield return null;
            }
        }
    }

    //Helper to wait until dialogue is done, then start @param dialogue, then run a callback like setting up a new event. 
    private IEnumerator StartDialogueWithNextEvent(List<DialogueText> dialogue, Action callbackToRun)
    {
        yield return new WaitUntil(() => !DialogueManager.Instance.IsInDialogue());
        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(dialogue));
        callbackToRun();
    }

}
