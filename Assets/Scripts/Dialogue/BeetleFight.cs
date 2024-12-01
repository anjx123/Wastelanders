
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;
using Cinemachine;
using UnityEngine.UI;
using Systems.Persistence;
using System;
using LevelSelectInformation;
//@author: Andrew
public class BeetleFight : DialogueClasses
{

    [SerializeField] private Jackie jackie;
    [SerializeField] private Transform jackieDefaultTransform;
    [SerializeField] private Transform jackieChasingTransform;
    [SerializeField] private Transform rightEntrance;
    [SerializeField] private Transform beetleRunsFromJackieTransform;
    [SerializeField] private Transform jackieSecondShot;
    [SerializeField] private Transform jackieReturnsToCampCameraPos;
    [SerializeField] private Transform campLeftEntrance;

    [SerializeField] private Transform beetleNestCameraPos;

    [SerializeField] private Ives ives;
    [SerializeField] private Transform ivesDefaultTransform;
    [SerializeField] private Transform ivesCombatTransform;
    [SerializeField] private Transform jackieCombatTransform;
    [SerializeField] private Transform ivesCombat2Transform;
    [SerializeField] private Transform jackieCombat2Transform;
    [SerializeField] private Transform ivesCombat3Transform;
    [SerializeField] private Transform jackieCombat3Transform;

    [SerializeField] private WasteFrog frog;
    [SerializeField] private WasteFrog frogThatRunsAway;

    [SerializeField] private Beetle ambushBeetle;
    [SerializeField] private Transform ambushBeetleTransform;
    [SerializeField] private Beetle wrangledBeetle;
    [SerializeField] private Beetle beetleDraggingCrystal;
    [SerializeField] private Crystals draggedCrystal;

    [SerializeField] private Image ivesVNSprite;
    [SerializeField] private Image jackieVNSprite;




    [SerializeField] private List<Transform> combatBeetleTransforms;

    [SerializeField] private ActionClass beetleAction;
    [SerializeField] private ActionClass jackieAction;


    [SerializeField] private GameObject theCampWithBeetles;
    [SerializeField] private GameObject beetleNest;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject wave1Fight;
    [SerializeField] private GameObject wave2Fight;
    [SerializeField] private GameObject wave3Fight;
    [SerializeField] private Crystals wave3Crystal;
    [SerializeField] private CinemachineVirtualCamera sceneCamera;
    [SerializeField] private Sprite frogDeathSprite;
    [SerializeField] private Sprite jackieCrystalSprite;
    [SerializeField] private GameOver gameOver;

    [SerializeField] private List<DialogueText> openingJackieQuipt;
    [SerializeField] private DialogueWrapper openingDiscussion;
    [SerializeField] private DialogueWrapper jackieSurprised;
    [SerializeField] private DialogueWrapper jackieChase;
    [SerializeField] private List<DialogueText> jackieWonAgainstBeetle;
    [SerializeField] private List<DialogueText> jackieLostAgainstBeetle;
    [SerializeField] private List<DialogueText> p2jackieWonAgainstBeetle;
    [SerializeField] private List<DialogueText> p2jackieLostAgainstBeetle;
    [SerializeField] private List<DialogueText> jackieDeclarationOfResolve;
    [SerializeField] private DialogueWrapper jackieBeetleCamp;
    [SerializeField] private List<DialogueText> jackieConcernedAboutBeetles;

    [SerializeField] private DialogueWrapper narratorCamp;
    [SerializeField] private DialogueWrapper jackieShockAtBeetleEnteringCamp;
    [SerializeField] private DialogueWrapper ivesConversation;
    [SerializeField] private DialogueWrapper twoPlayerCombatTutorial;
    [SerializeField] private DialogueWrapper ivesTutorial;
    [SerializeField] private DialogueWrapper wave2Dialogue;
    [SerializeField] private DialogueWrapper wave3Dialogue;
    [SerializeField] private DialogueWrapper postBattleDialogue; 
    [SerializeField] private DialogueWrapper gameLoseDialogue;
    [SerializeField] private DialogueWrapper resonateExplanation;
    [SerializeField] private DialogueWrapper crystalExplanation;

    [SerializeField] private List<DialogueText> afterWave3Dialogue;
    [SerializeField] private List<DialogueText> outOfCombatCrystalDialogue;
    [SerializeField] private List<DialogueText> jackieFeelsStronger;
    [SerializeField] private List<DialogueText> ivesFeelsStronger;
    [SerializeField] private List<DialogueText> ivesAnalyzeAfterExam;


    [SerializeField] private bool jumpToCombat;

    private List<EnemyClass> campBeetlesAndCrystals = new();
    private List<EnemyClass> nitesCampBeetles = new();
    List<EnemyClass> wave1 = new();
    List<EnemyClass> wave2 = new();
    List<EnemyClass> wave3 = new();



    private const float BRIEF_PAUSE = 0.2f; // For use after an animation to make it visually seem smoother
    private const float MEDIUM_PAUSE = 1f; //For use after a text box comes down and we want to add some weight to the text.

    private void OnDestroy()
    {
        HighlightManager.Instance.EntityClicked -= EntityClicked;
        CombatManager.PlayersWinEvent -= AllEntitiesDied;
        CombatManager.EnemiesWinEvent -= EnemiesWin;

        Beetle.OnGainBuffs -= ExplainCrystals;

        jackie.BuffsUpdatedEvent -= ExplainPlayerBuffed;
        ives.BuffsUpdatedEvent -= ExplainPlayerBuffed;

        Beetle.OnGainBuffs -= ExplainResonate;


        CombatManager.ClearEvents();
        DialogueBox.ClearDialogueEvents();
    }
    protected override void GameStateChange(GameState gameState)
    {
        if (gameState == GameState.GAME_START)
        {
            StartCoroutine(ExecuteGameStart());
        }
    }

    private void SetUpEnemyLists()
    {
        foreach (Transform entity in beetleNest.transform)
        {
            campBeetlesAndCrystals.Add(entity.GetComponent<EnemyClass>());
            entity.GetComponent<EnemyClass>().OutOfCombat();
        }
        foreach (Transform entity in theCampWithBeetles.transform)
        {
            nitesCampBeetles.Add(entity.GetComponent<EnemyClass>());
            entity.GetComponent<EnemyClass>().OutOfCombat();
            entity.GetComponent<EnemyClass>().animator.enabled = false;
        }
        foreach (Transform enemy in wave1Fight.transform)
        {
            EnemyClass enemyBeetle = enemy.GetComponent<EnemyClass>();
            wave1.Add(enemyBeetle);
        }
        foreach (Transform enemy in wave2Fight.transform)
        {
            EnemyClass enemyBeetle = enemy.GetComponent<EnemyClass>();
            wave2.Add(enemyBeetle);
        }
        foreach (Transform enemy in wave3Fight.transform)
        {
            EnemyClass enemyBeetle = enemy.GetComponent<EnemyClass>();
            wave3.Add(enemyBeetle);
        }
        CombatManager.Instance.SetEnemiesPassive(wave1);
        CombatManager.Instance.SetEnemiesPassive(wave2);
        CombatManager.Instance.SetEnemiesPassive(wave3);
    }

    private void SetUpCombatStatus()
    {
        draggedCrystal.OutOfCombat();
        beetleNest.SetActive(false);
        theCampWithBeetles.SetActive(false);
        ives.OutOfCombat();
        jackie.OutOfCombat(); //Workaround for now, ill have to remove this once i manually start instantiating players
        frog.OutOfCombat();
        frog.FaceLeft();
        frogThatRunsAway.OutOfCombat();
        frogThatRunsAway.FaceLeft();
        ambushBeetle.OutOfCombat();
        wrangledBeetle.OutOfCombat();
        jackie.SetReturnPosition(jackieCombatTransform.position);
        ives.SetReturnPosition(ivesCombatTransform.position);
    }

    private IEnumerator ExecuteGameStart()
    {
        CombatManager.Instance.GameState = GameState.OUT_OF_COMBAT;
        CombatManager.Instance.SetDarkScreen();
        yield return new WaitForSeconds(0.2f);
        SetUpEnemyLists();
        SetUpCombatStatus();
        if (!jumpToCombat && !GameStateManager.Instance.JumpIntoBeetleFight)
        {
            sceneCamera.Priority = 2;
            yield return new WaitForSeconds(1f);
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
                yield return StartCoroutine(DialogueManager.Instance.StartDialogue(openingJackieQuipt));
            }

            // BEETLE AMBUSH!!
            {
                void ShowCrystal()
                {
                    jackie.FaceRight();
                    jackie.animator.enabled = false;
                    jackie.GetComponent<SpriteRenderer>().sprite = jackieCrystalSprite;
                    DialogueBox.DialogueBoxEvent -= ShowCrystal;
                }
                DialogueBox.DialogueBoxEvent += ShowCrystal;
                yield return new WaitForSeconds(BRIEF_PAUSE);
                yield return StartCoroutine(jackie.MoveToPosition(frog.transform.position, 1.2f, 1f));
                sceneCamera.transform.position = CombatManager.Instance.dynamicCamera.transform.position;
                yield return new WaitForSeconds(BRIEF_PAUSE);
                sceneCamera.Priority = 2;
                yield return new WaitForSeconds(BRIEF_PAUSE);
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
                jackie.FaceLeft();
                yield return StartCoroutine(DialogueManager.Instance.StartDialogue(jackieSurprised.Dialogue));
                jackie.animator.enabled = true;

                sceneCamera.Priority = 0;
                yield return StartCoroutine(CardComparator.Instance.ClashCards(jackieAction, beetleAction));
                DialogueBox.DialogueBoxEvent -= ShowCrystal; //in case some weird shit happens
            }

            //Jackie Chases the beetle and we fade
            {
                sceneCamera.transform.position = CombatManager.Instance.dynamicCamera.transform.position;
                yield return StartCoroutine(ambushBeetle.MoveToPosition(jackieChasingTransform.position, 0, 1.2f)); // beetle runs away

                jackie.DeEmphasize();
                sceneCamera.Priority = 2;
                CombatManager.Instance.ActivateBaseCamera();
                yield return StartCoroutine(DialogueManager.Instance.StartDialogue(jackieChase.Dialogue));
                Coroutine fade = StartCoroutine(CombatManager.Instance.FadeInDarkScreen(1f));
                yield return StartCoroutine(jackie.MoveToPosition(jackieChasingTransform.position, 1f, 1f)); //Jackie Runs into the scene
                yield return fade;

            }

            //Jackie Vows to hunt down that beetle 
            {
                RemoveEnemyFromScene(frog);
                sceneCamera.m_Lens.OrthographicSize = 5f;
                sceneCamera.transform.position = beetleNestCameraPos.transform.position;

                CombatManager.Instance.ActivateBaseCamera();
                yield return new WaitForSeconds(MEDIUM_PAUSE);
                if (jackieAction.getRolledDamage() >= beetleAction.getRolledDamage())
                {
                    yield return StartCoroutine(DialogueManager.Instance.StartDialogue(jackieWonAgainstBeetle));

                }
                else
                {
                    yield return StartCoroutine(DialogueManager.Instance.StartDialogue(jackieLostAgainstBeetle));
                }
                yield return StartCoroutine(DialogueManager.Instance.StartDialogue(jackieDeclarationOfResolve));
                jackie.transform.position = rightEntrance.position;
                ambushBeetle.transform.position = rightEntrance.position;
            }

            //Jackie chases after beetle who might have stolen her crystal
            {
                beetleNest.SetActive(true);
                yield return StartCoroutine(CombatManager.Instance.FadeInLightScreen(1.2f));
                Coroutine beetleRunsIn = StartCoroutine(ambushBeetle.MoveToPosition(beetleRunsFromJackieTransform.position, 0f, 2.5f));
                yield return new WaitForSeconds(0.7f);
                yield return StartCoroutine(jackie.MoveToPosition(jackieSecondShot.position, 0f, 1f));
                yield return new WaitForSeconds(BRIEF_PAUSE);
                jackie.AttackAnimation("IsShooting");
                StopCoroutine(beetleRunsIn);
                yield return StartCoroutine(ambushBeetle.StaggerEntities(jackie, ambushBeetle, 0.3f));
                DieInScene(ambushBeetle);
                yield return new WaitForSeconds(BRIEF_PAUSE);
                if (jackieAction.getRolledDamage() >= beetleAction.getRolledDamage())
                {
                    jackieConcernedAboutBeetles.RemoveAt(jackieConcernedAboutBeetles.Count - 1);
                    yield return StartCoroutine(DialogueManager.Instance.StartDialogue(p2jackieWonAgainstBeetle));
                }
                else
                {
                    yield return StartCoroutine(DialogueManager.Instance.StartDialogue(p2jackieLostAgainstBeetle));
                }
                yield return StartCoroutine(jackie.MoveToPosition(ambushBeetle.transform.position, 1.2f, 1f));
                yield return new WaitForSeconds(BRIEF_PAUSE);

                Coroutine dragJob = StartCoroutine(beetleDraggingCrystal.MoveToPosition(beetleDraggingCrystal.transform.position + new Vector3(-8, 0, 0), 0, 10f, rightEntrance.position));
                yield return StartCoroutine(ShiftObjectCoroutine(sceneCamera.gameObject, 8f, 2f));
                yield return StartCoroutine(DialogueManager.Instance.StartDialogue(jackieBeetleCamp.Dialogue));
                yield return StartCoroutine(jackie.MoveToPosition(rightEntrance.transform.position, 1.2f, 1f));
                yield return StartCoroutine(CombatManager.Instance.FadeInDarkScreen(0.6f));
                StopCoroutine(dragJob);
                jackie.Heal(5);
            }

            //Jackie sees the nest of beetles
            {
                beetleNest.SetActive(false);
                RemoveEnemyFromScene(ambushBeetle);
                sceneCamera.Priority = 2;
                sceneCamera.transform.position = jackieReturnsToCampCameraPos.position;
                sceneCamera.m_Lens.OrthographicSize = 3.5f;

                ives.transform.position = ivesDefaultTransform.position;
                yield return StartCoroutine(DialogueManager.Instance.StartDialogue(jackieConcernedAboutBeetles));
                yield return StartCoroutine(DialogueManager.Instance.StartDialogue(narratorCamp.Dialogue));
            }

            //Jackie comes to camp and sees a bunch of beetles/some dead 
            {
                jackie.transform.position = campLeftEntrance.position;
                theCampWithBeetles.SetActive(true);
                wrangledBeetle.animator.enabled = true;
                yield return StartCoroutine(CombatManager.Instance.FadeInLightScreen(0.6f));
                yield return StartCoroutine(jackie.MoveToPosition(campLeftEntrance.position + new Vector3(2f, 0, 0), 0f, 1f));
                yield return StartCoroutine(DialogueManager.Instance.StartDialogue(jackieShockAtBeetleEnteringCamp.Dialogue));
                sceneCamera.Priority = 0;
                CombatManager.Instance.ActivateDynamicCamera();
                yield return StartCoroutine(jackie.MoveToPosition(ivesDefaultTransform.position, 1.5f, 2.5f));
                yield return StartCoroutine(DialogueManager.Instance.StartDialogue(ivesConversation.Dialogue));

                Coroutine ivesReset = StartCoroutine(ives.ResetPosition());
                yield return StartCoroutine(jackie.ResetPosition());
                yield return ivesReset;
                yield return new WaitForSeconds(0.5f);
            }

        }
        else // setup scene
        {
            GameStateManager.Instance.JumpIntoBeetleFight = false;
            RemoveEnemyFromScene(frog);
            RemoveEnemyFromScene(frogThatRunsAway);
            RemoveEnemyFromScene(ambushBeetle);
            ives.transform.position = ivesCombatTransform.position + new Vector3(-4, 0, 0);
            Coroutine fade = StartCoroutine(CombatManager.Instance.FadeInLightScreen(2f));
        }

        //kill off the other entities in the scene
        {
            theCampWithBeetles.SetActive(true);
            beetleNest.SetActive(true);
            foreach (EnemyClass entity in campBeetlesAndCrystals)
            {
                DieInScene(entity);
                Destroy(entity.gameObject);
            }
            foreach (EnemyClass entity in nitesCampBeetles)
            {
                DieInScene(entity);
                Destroy(entity.gameObject);
            }
            DieInScene(draggedCrystal);
            Destroy(draggedCrystal.gameObject);
        }

        //Start wave 1 
        {
            wave2Fight.SetActive(false);
            wave3Fight.SetActive(false);
            CombatManager.Instance.SetEnemiesHostile(wave1);
            jackie.InCombat();
            ives.InCombat();
            jackie.Targetable();
            ives.Targetable();
            DialogueManager.Instance.MoveBoxToTop();

            yield return new WaitForSeconds(0.2f);
            CombatManager.Instance.GameState = GameState.SELECTION;
            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(twoPlayerCombatTutorial.Dialogue));
            Begin2PCombatTutorial();
            yield return new WaitUntil(() => CombatManager.Instance.GameState == GameState.GAME_WIN);
        }

        //Start wave 2
        {
            wave2Fight.SetActive(true);
            CombatManager.Instance.SetEnemiesHostile(wave2);
            
            if (!jackie.IsDead)
            {
                jackie.SetReturnPosition(jackieCombat2Transform.position);
            }
            if (!ives.IsDead)
            {
                ives.SetReturnPosition(ivesCombat2Transform.position);
            }
            yield return ShiftObjectCoroutine(CombatManager.Instance.baseCamera.gameObject, -7.5f, 3f);
            yield return new WaitForSeconds(0.5f);
            CombatManager.Instance.GameState = GameState.SELECTION;
            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(wave2Dialogue.Dialogue));
            BeginWave2();
            yield return new WaitUntil(() => CombatManager.Instance.GameState == GameState.GAME_WIN);
        }

        //Start wave 3
        {
            wave3Fight.SetActive(true);
            CombatManager.Instance.SetEnemiesHostile(wave3);
            if (!jackie.IsDead)
            {
                jackie.SetReturnPosition(jackieCombat3Transform.position);
            }
            if (!ives.IsDead)
            {
                ives.SetReturnPosition(ivesCombat3Transform.position);
            }
            yield return ShiftObjectCoroutine(CombatManager.Instance.baseCamera.gameObject, -9.5f, 3f);
            yield return new WaitForSeconds(0.5f);
            CombatManager.Instance.GameState = GameState.SELECTION;
            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(wave3Dialogue.Dialogue));
            BeginWave3();
            yield return new WaitUntil(() => CombatManager.Instance.GameState == GameState.GAME_WIN);
        }

        //End of Scene
        {
            yield return new WaitForSeconds(2);
            if (ives.IsDead)
            {
                ives.gameObject.SetActive(true);
                ives.OutOfCombat();
                ives.SetReturnPosition(ivesCombat3Transform.transform.position);
                StartCoroutine(ives.ResetPosition());
            }

            if (jackie.IsDead)
            {
                jackie.gameObject.SetActive(true);
                jackie.OutOfCombat();
                jackie.SetReturnPosition(jackieCombat3Transform.transform.position);
                StartCoroutine(jackie.ResetPosition());
            }
            DialogueManager.Instance.MoveBoxToBottom();
            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(afterWave3Dialogue));
            yield return new WaitForSeconds(BRIEF_PAUSE);

            if (!wave3Crystal.IsDead)
            {
                yield return new WaitForSeconds(BRIEF_PAUSE);
                yield return StartCoroutine(jackie.MoveToPosition(wave3Crystal.transform.position, 1.2f, 1f));
            }

            yield return StartCoroutine(OutOfCombatCrystalDialogue());
            if (!wave3Crystal.IsDead)
            {
                jackie.AttackAnimation("IsStaffing");
                wave3Crystal.TakeDamage(jackie, 5);
                yield return new WaitForSeconds(MEDIUM_PAUSE);
            }

            yield return new WaitUntil(() => !DialogueManager.Instance.IsInDialogue());
            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(ivesAnalyzeAfterExam));

            CombatManager.Instance.GameState = GameState.OUT_OF_COMBAT;
            jackie.OutOfCombat();
            ives.OutOfCombat();
            jackie.FaceLeft();
            ives.FaceRight();

            jackieVNSprite.gameObject.SetActive(true);
            ivesVNSprite.gameObject.SetActive(true);


            DialogueBox.DialogueBoxEvent += JackieMentionsSheSawBeetles;
            void JackieMentionsSheSawBeetles() {
                DialogueBox.DialogueBoxEvent -= JackieMentionsSheSawBeetles;
                StartCoroutine(MoveToRight(jackieVNSprite.gameObject.GetComponent<RectTransform>(), 400f, 0.8f));
            }

            yield return new WaitUntil(() => !DialogueManager.Instance.IsInDialogue());
            yield return StartCoroutine(CombatManager.Instance.FadeInDarkScreen(2f));
            yield return StartCoroutine(FadeImage(ivesVNSprite, 1.1f, true));
            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(postBattleDialogue.Dialogue));

            Coroutine ivesFade = StartCoroutine(FadeImage(ivesVNSprite, 1.5f, false));
            AudioManager.Instance.FadeOutCurrentBackgroundTrack(2f);
            yield return StartCoroutine(FadeImage(jackieVNSprite, 1.5f, false));
            yield return ivesFade;
            yield return new WaitForSeconds(MEDIUM_PAUSE);

            GameStateManager.Instance.JustFinishedBeetleFight = true;
            GameStateManager.Instance.CurrentLevelProgress = Math.Max(GameStateManager.Instance.CurrentLevelProgress, StageInformation.BEETLE_STAGE.LevelID + 1f);
            GameStateManager.Instance.LoadScene(GameStateManager.SELECTION_SCREEN_NAME);
            yield break;
        }
    }

    private void Begin2PCombatTutorial()
    {
        HighlightManager.Instance.EntityClicked += EntityClicked;
        CombatManager.PlayersWinEvent += AllEntitiesDied;
        CombatManager.EnemiesWinEvent += EnemiesWin;
    }
    private void BeginWave2()
    {
        CombatManager.PlayersWinEvent += AllEntitiesDied;
        CombatManager.EnemiesWinEvent += EnemiesWin;
        Beetle.OnGainBuffs += ExplainResonate;
    }

    private void BeginWave3()
    {
        CombatManager.PlayersWinEvent += AllEntitiesDied;
        CombatManager.EnemiesWinEvent += EnemiesWin;
        Beetle.OnGainBuffs += ExplainCrystals;

        jackie.BuffsUpdatedEvent += ExplainPlayerBuffed;
        ives.BuffsUpdatedEvent += ExplainPlayerBuffed;
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
        HighlightManager.Instance.EntityClicked -= EntityClicked;
        yield return new WaitUntil(() => (!DialogueManager.Instance.IsInDialogue()));
        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(ivesTutorial.Dialogue));
    }


    private void ExplainResonate(string buffType, int stacks, Beetle beetle)
    {
        if (buffType == Resonate.buffName)
        {
            StartCoroutine(ExplainResonateDialogue());
        }
    }
    private IEnumerator ExplainResonateDialogue()
    {
        Beetle.OnGainBuffs -= ExplainResonate;
        yield return new WaitUntil(() => !DialogueManager.Instance.IsInDialogue() && CombatManager.Instance.GameState != GameState.FIGHTING);
        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(resonateExplanation.Dialogue));
    }
    //If a beetle other than a drone has gained stacks, that means they broke a crystal
    private void ExplainCrystals(string buffType, int stacks, Beetle beetle)
    {
        if (buffType == Resonate.buffName && beetle is not DroneBeetle)
        {
            StartCoroutine(ExplainCrystalsDialogue());
        }
    }

    private IEnumerator ExplainCrystalsDialogue()
    {
        Beetle.OnGainBuffs -= ExplainCrystals;
        yield return new WaitUntil(() => !DialogueManager.Instance.IsInDialogue() && CombatManager.Instance.GameState != GameState.FIGHTING);
        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(crystalExplanation.Dialogue));
    }

    private IEnumerator OutOfCombatCrystalDialogue()
    {
        Beetle.OnGainBuffs -= ExplainCrystals;
        yield return new WaitUntil(() => !DialogueManager.Instance.IsInDialogue() && CombatManager.Instance.GameState != GameState.FIGHTING);
        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(outOfCombatCrystalDialogue));
    }

    private void ExplainPlayerBuffed(EntityClass entity)
    {
        if (entity.GetBuffStacks(Resonate.buffName) > 0)
        {
            StartCoroutine(ExplainPlayerBuffedDialogue(entity));
        }
    }

    private IEnumerator ExplainPlayerBuffedDialogue(EntityClass entity)
    {

        jackie.BuffsUpdatedEvent -= ExplainPlayerBuffed;
        ives.BuffsUpdatedEvent -= ExplainPlayerBuffed;
        yield return new WaitUntil(() => !DialogueManager.Instance.IsInDialogue() && CombatManager.Instance.GameState != GameState.FIGHTING);
        if (entity is Jackie)
        {
            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(jackieFeelsStronger));
        } else
        {
            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(ivesFeelsStronger));
        }
    }

    private void AllEntitiesDied()
    {
        CombatManager.Instance.GameState = GameState.GAME_WIN;
        CombatManager.PlayersWinEvent -= AllEntitiesDied;
        CombatManager.EnemiesWinEvent -= EnemiesWin;
    }


    private void EnemiesWin()
    {
        CombatManager.EnemiesWinEvent -= EnemiesWin;
        CombatManager.PlayersWinEvent -= AllEntitiesDied;
        HighlightManager.Instance.EntityClicked -= EntityClicked;
        Beetle.OnGainBuffs -= ExplainCrystals;
        Beetle.OnGainBuffs -= ExplainResonate;
        jackie.BuffsUpdatedEvent -= ExplainPlayerBuffed;
        ives.BuffsUpdatedEvent -= ExplainPlayerBuffed;
        StartCoroutine(GameLose());
        CombatManager.Instance.GameState = GameState.GAME_LOSE;
    }
    private IEnumerator GameLose()
    {
        yield return StartCoroutine(CombatManager.Instance.FadeInDarkScreen(2f));

        DialogueManager.Instance.MoveBoxToBottom();
        GameStateManager.Instance.JumpIntoBeetleFight = true;
        gameOver.gameObject.SetActive(true);
        gameOver.FadeIn();

        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(gameLoseDialogue.Dialogue));
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

    IEnumerator MoveToRight(RectTransform rectTransform, float moveAmount, float duration)
    {
        float counter = 0;
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 endPos = new Vector2(startPos.x + moveAmount, startPos.y);

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float t = counter / duration;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }
    }

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

    // removes the entity from combat
    private void RemoveEnemyFromScene(EnemyClass e)
    {
        BattleQueue.BattleQueueInstance.RemoveAllInstancesOfEntity(e);
        CombatManager.Instance.RemoveEnemy(e);
        DieInScene(e);
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
        DieInScene(wasteFrog);
        wasteFrog.gameObject.SetActive(false);
    }
    void DieInScene(EnemyClass enemyEntity)
    {
        BattleQueue.BattleQueueInstance.RemoveAllInstancesOfEntity(enemyEntity);
        CombatManager.Instance.RemoveEnemy(enemyEntity);
        enemyEntity.animator.enabled = false;
        if (enemyEntity is WasteFrog)
        {
            enemyEntity.GetComponent<SpriteRenderer>().sprite = frogDeathSprite;
        }
        enemyEntity.GetComponent<SpriteRenderer>().sortingOrder -= 1;
        enemyEntity.OutOfCombat();
        enemyEntity.UnTargetable();
        enemyEntity.combatInfo.gameObject.SetActive(false);
        enemyEntity.transform.rotation = Quaternion.Euler(0, 0, 75);
        enemyEntity.DestroyDeck();
    }
}
