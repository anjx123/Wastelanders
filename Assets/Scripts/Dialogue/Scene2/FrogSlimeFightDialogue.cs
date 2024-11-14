using Cinemachine;
using LevelSelectInformation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Systems.Persistence;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Beetle;

public class FrogSlimeFightDialogue : DialogueClasses
{
    [SerializeField] private Jackie jackie;
    [SerializeField] private Transform jackieDefaultTransform;
    [SerializeField] private Transform jackieWander1;
    [SerializeField] private Transform jackieWander2;
    [SerializeField] private Transform jackieWander3;
    [SerializeField] private Transform outOfScreen;
    [SerializeField] private Transform treeHidingPositionJackie;
    [SerializeField] private Transform jackieFiresShotTransform;

    [SerializeField] private WasteFrog frog;
    [SerializeField] private Transform frogInitialWalkIn;
    [SerializeField] private Transform frogConfrontPosition;
    [SerializeField] private Transform frogFightPosition;

    [SerializeField] private WasteFrog frog2;
    [SerializeField] private Transform frog2Battle;
    [SerializeField] private Transform frog2WalkIn;

    [SerializeField] private SlimeStack slime;
    [SerializeField] private Transform slimeBattle;
    [SerializeField] private Transform slimeWalkIn;

    [SerializeField] private GameOver gameOver;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject scoutBeetlePrefab;
    [SerializeField] CinemachineVirtualCamera closeUpCamera;
    [SerializeField] Image ivesImage;
    [SerializeField] private SpriteRenderer treeOverlay;
    [SerializeField] private Sprite frogDeathSprite;
    [SerializeField] private Sprite jackieHoldingCrystalSprite;

    [SerializeField] private List<DialogueText> sceneNarration;
    [SerializeField] private List<DialogueText> ivesInstruction;
    [SerializeField] private List<DialogueText> jackieStrategyPlan;
    // After the frog enters the scene
    [SerializeField] private List<DialogueText> jackiePreMissedShot;
    [SerializeField] private List<DialogueText> jackiePostMissedShot;
    [SerializeField] private List<DialogueText> jackiePreCombat;
    //Combat Dialogue
    [SerializeField] private List<DialogueText> startOfCombatDialogue;
    // After the frog is defeated
    [SerializeField] private List<DialogueText> afterCombatDialogue;
    [SerializeField] private List<DialogueText> crystalExtraction;
    [SerializeField] private List<DialogueText> beetleEntrance;


    //Game Lose Dialogue
    [SerializeField] private List<DialogueText> gameLoseDialogue;


    [SerializeField] private bool jumpToCombat;



    private bool playDeadFrog = false;
    private WasteFrog lastKilledFrog;



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
        EntityClass.OnEntityDeath -= EnsureFrogDeath;
    }

    int numberOfBroadcasts = 0;
private IEnumerator ExecuteGameStart()
    {
        CombatManager.Instance.GameState = GameState.OUT_OF_COMBAT;
        CombatManager.Instance.SetDarkScreen();
        yield return new WaitForSeconds(0.8f);

        jackie.OutOfCombat();
        frog.OutOfCombat();
        frog2.OutOfCombat();
        slime.OutOfCombat();
        
        jackie.SetReturnPosition(jackieDefaultTransform.position);
        frog.SetReturnPosition(frogFightPosition.position);
        frog2.SetReturnPosition(frog2Battle.position);
        slime.SetReturnPosition(slimeBattle.position);

        frog.UnTargetable(); frog2.UnTargetable(); slime.UnTargetable();
        if (!jumpToCombat && !GameStateManager.Instance.JumpIntoFrogAndSlimeFight)
        {

            //Narrate the scene
            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(sceneNarration));
            yield return new WaitForSeconds(BRIEF_PAUSE);

            //Ives Talks to the examinees
            yield return StartCoroutine(FadeImage(ivesImage, 1f, true));
            yield return new WaitForSeconds(BRIEF_PAUSE);
            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(ivesInstruction));
            yield return StartCoroutine(FadeImage(ivesImage, 1f, false));
            ivesImage.gameObject.SetActive(false);

            //Jackie walks in the scene
            jackie.Emphasize();
            yield return StartCoroutine(jackie.MoveToPosition(jackieDefaultTransform.position, 0f, 1.2f));
            yield return new WaitForSeconds(BRIEF_PAUSE);
            yield return StartCoroutine(CombatManager.Instance.FadeInLightScreen(1.5f));
            jackie.DeEmphasize();

            //Jackie Wanders around
            DialogueBox.DialogueBoxEvent += OnDialogueBoxEvent;
            Coroutine jackieWander = StartCoroutine(HaveJackieWander());
            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(jackieStrategyPlan));
            yield return jackieWander;
            yield return StartCoroutine(WalkWhileScreenFades());
            //Jackie Hides behind tree
            closeUpCamera.Priority = 2;
            yield return new WaitForSeconds(1f);
            jackie.GetComponent<SpriteRenderer>().sortingOrder = treeOverlay.sortingOrder - 1;
            jackie.gameObject.transform.position = treeHidingPositionJackie.position;
            jackie.gameObject.transform.rotation = treeHidingPositionJackie.rotation;
            jackie.animator.enabled = false;
            yield return StartCoroutine(CombatManager.Instance.FadeInLightScreen(2f));

            // Frog walks in
            StartCoroutine(frog.MoveToPosition(frogInitialWalkIn.position, 0f, 2f));
            yield return new WaitForSeconds(1f);
            yield return StartCoroutine(MoveObjectInRotationDirection(jackie.gameObject, 0.3f, 0.3f));
            yield return new WaitForSeconds(BRIEF_PAUSE);
            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(jackiePreMissedShot));

            //Jackie Misses and frog runs away with jackie chasing it
            yield return new WaitForSeconds(1f);
            jackie.gameObject.transform.rotation = new Quaternion(0,0,0,0);
            jackie.gameObject.transform.position = treeHidingPositionJackie.position + new Vector3(0.8f, 0, 0);
            jackie.animator.enabled = true;
            jackie.GetComponent<SpriteRenderer>().sortingOrder = treeOverlay.sortingOrder + 1;
            jackie.AttackAnimation("IsShooting");
            yield return StartCoroutine(MakeFrogJump(frog, 1f));
            yield return StartCoroutine(frog.MoveToPosition(frogConfrontPosition.position, 0f, 1.2f, outOfScreen.position));
            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(jackiePostMissedShot));
            closeUpCamera.Priority = 0;
            yield return StartCoroutine(jackie.MoveToPosition(frogConfrontPosition.position, 2f, 1.6f));
            yield return new WaitForSeconds(MEDIUM_PAUSE);
            frog.FaceLeft();
            
            //Other Enemies walk in
            StartCoroutine(frog2.MoveToPosition(frog2WalkIn.position, 0f, 2f));
            yield return StartCoroutine(slime.MoveToPosition(slimeWalkIn.position, 0f, 2f));
            yield return new WaitForSeconds(MEDIUM_PAUSE);
            
            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(jackiePreCombat));
        } else
        {
            GameStateManager.Instance.JumpIntoFrogAndSlimeFight = false;
        }

        // start frog fight
        treeOverlay.enabled = false;
        StartCoroutine(jackie.ResetPosition());
        StartCoroutine(frog2.ResetPosition());
        StartCoroutine(slime.ResetPosition());
        frog.SetReturnPosition(frogFightPosition.position);
        yield return StartCoroutine(frog.ResetPosition());
        jackie.InCombat();
        frog.Targetable(); frog.InCombat(); frog2.Targetable(); frog2.InCombat(); slime.Targetable(); slime.InCombat();
        yield return new WaitUntil(() => !DialogueManager.Instance.IsInDialogue());

        CombatManager.PlayersWinEvent += PlayersWin;
        CombatManager.EnemiesWinEvent += EnemiesWin;
        EntityClass.OnEntityDeath += EnsureFrogDeath;

        CombatManager.Instance.GameState = GameState.SELECTION;
        
        //Starting Combat
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(startOfCombatDialogue));
        yield return new WaitUntil(() => CombatManager.Instance.GameState == GameState.GAME_WIN);
        CombatManager.Instance.GameState = GameState.OUT_OF_COMBAT;
        yield return new WaitForSeconds(MEDIUM_PAUSE);

        //After Combat
        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(afterCombatDialogue));
        yield return new WaitForSeconds(BRIEF_PAUSE);
        CombatManager.Instance.ActivateDynamicCamera();
        yield return StartCoroutine(jackie.MoveToPosition(lastKilledFrog.transform.position, 1.5f, 1.7f));
        yield return new WaitForSeconds(MEDIUM_PAUSE);
        closeUpCamera.transform.position = CombatManager.Instance.dynamicCamera.transform.position;
        closeUpCamera.m_Lens.OrthographicSize = CombatManager.Instance.dynamicCamera.m_Lens.OrthographicSize;

        //Jackei picks up the crystal
        jackie.animator.enabled = false;
        jackie.transform.rotation = Quaternion.Euler(0, 0, -25);
        yield return new WaitForSeconds(MEDIUM_PAUSE);
        jackie.GetComponent<SpriteRenderer>().sprite = jackieHoldingCrystalSprite;
        yield return new WaitForSeconds(MEDIUM_PAUSE);
        jackie.transform.rotation = Quaternion.identity;
        yield return new WaitForSeconds(BRIEF_PAUSE);
        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(crystalExtraction));

        //Jackie moves off screen
        closeUpCamera.Priority = 2;
        jackie.animator.enabled = true;
        jackie.Emphasize();
        yield return new WaitForSeconds(MEDIUM_PAUSE);
        yield return StartCoroutine(jackie.MoveToPosition(jackie.transform.position + new Vector3(12f, -1f, 0), 0f, 1.5f));
        yield return new WaitForSeconds(MEDIUM_PAUSE);

        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(beetleEntrance));
        yield return new WaitForSeconds(BRIEF_PAUSE);
        //Beetle is spawned in and follows Jackie
        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0.6f, mainCamera.nearClipPlane));
        GameObject scoutBeetleObj = Instantiate(scoutBeetlePrefab, bottomLeft + new Vector3(-0.32f, 0, 0), Quaternion.identity);
        ScoutBeetle scoutBeetle = scoutBeetleObj.GetComponent<ScoutBeetle>();
        scoutBeetle.OutOfCombat();
        scoutBeetle.UnTargetable();
        yield return new WaitForSeconds(2f);
        StartCoroutine(scoutBeetle.MoveToPosition(jackie.transform.position, 0f, 2.5f));

        AudioManager.Instance.FadeOutCurrentBackgroundTrack(2f);
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(CombatManager.Instance.FadeInDarkScreen(1.5f));

        GameStateManager.Instance.CurrentLevelProgress = Math.Max(GameStateManager.Instance.CurrentLevelProgress, StageInformation.FROG_SLIME_STAGE.LevelID + 1f);

        GameStateManager.Instance.LoadScene(GameStateManager.BEETLE_FIGHT);
        yield break;
    }

    private IEnumerator WalkWhileScreenFades()
    {
        IEnumerator i1 = jackie.MoveToPosition(outOfScreen.position, 0f, 2.2f);
        IEnumerator i2 = CombatManager.Instance.FadeInDarkScreen(2f);
        StartCoroutine(i1);
        yield return StartCoroutine(i2);
        yield break;
    }
    IEnumerator HaveJackieWander()
    {
        //Jackie wanders up
        yield return new WaitUntil(() => numberOfBroadcasts >= 1);
        yield return StartCoroutine(jackie.MoveToPosition(jackieWander1.position, 0f, 1.2f));
        yield return new WaitForSeconds(BRIEF_PAUSE);

        //jackie Shakes her head
        yield return new WaitUntil(() => numberOfBroadcasts >= 2);
        jackie.FaceLeft();
        yield return new WaitForSeconds(0.3f);
        jackie.FaceRight();
        yield return new WaitForSeconds(0.3f);
        jackie.FaceLeft();
        yield return new WaitForSeconds(0.3f);
        jackie.FaceRight();

        //Jackie wanders down
        yield return new WaitUntil(() => numberOfBroadcasts >= 3);
        yield return StartCoroutine(jackie.MoveToPosition(jackieWander2.position, 0f, 1f));
        yield return new WaitForSeconds(BRIEF_PAUSE);

        //Jackie wanders to the middle
        yield return new WaitUntil(() => numberOfBroadcasts >= 4);
        yield return StartCoroutine(jackie.MoveToPosition(jackieWander3.position, 0f, 0.8f));
        yield return new WaitForSeconds(BRIEF_PAUSE);

        //jackie turns left
        yield return new WaitUntil(() => numberOfBroadcasts >= 5);
        jackie.FaceLeft();
        DialogueBox.DialogueBoxEvent -= OnDialogueBoxEvent;
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
    IEnumerator MoveObjectInRotationDirection(GameObject obj, float distance, float duration)
    {
        Vector3 startPosition = obj.transform.position;
        Vector3 endPosition = obj.transform.position + obj.transform.up * distance;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            obj.transform.position = Vector3.Lerp(startPosition, endPosition, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the object ends up at the exact end position
        obj.transform.position = endPosition;
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

    private void OnDialogueBoxEvent()
    {
        ++numberOfBroadcasts;
    }
    private void EnsureFrogDeath(EntityClass entity)
    {
        if (entity is WasteFrog wasteFrog) 
        {
            if (playDeadFrog)
            {
                //Function that overrides death animation to die in the scene
                IEnumerator DieInScene()
                {
                    BattleQueue.BattleQueueInstance.RemoveAllInstancesOfEntity(wasteFrog);
                    CombatManager.Instance.RemoveEnemy(wasteFrog);
                    wasteFrog.animator.enabled = false;
                    wasteFrog.GetComponent<SpriteRenderer>().sprite = frogDeathSprite;
                    wasteFrog.OutOfCombat();
                    wasteFrog.UnTargetable();
                    wasteFrog.combatInfo.gameObject.SetActive(false);
                    wasteFrog.transform.rotation = Quaternion.Euler(0, 0, 75);
                    wasteFrog.DestroyDeck();
                    yield break;
                }

                wasteFrog._DeathHandler = DieInScene;
                lastKilledFrog = wasteFrog;
                EntityClass.OnEntityDeath -= EnsureFrogDeath;
            }
            playDeadFrog = true; //Only change death animation of the second frog
        }
    }




    


    private void PlayersWin()
    {
        CombatManager.EnemiesWinEvent -= EnemiesWin;
        CombatManager.PlayersWinEvent -= PlayersWin;
        CombatManager.Instance.GameState = GameState.GAME_WIN;
    }

    private void EnemiesWin()
    {
        CombatManager.EnemiesWinEvent -= EnemiesWin;
        CombatManager.PlayersWinEvent -= PlayersWin;
        StartCoroutine(GameLose());
        CombatManager.Instance.GameState = GameState.GAME_LOSE;
    }
    private IEnumerator GameLose()
    {
        yield return StartCoroutine(CombatManager.Instance.FadeInDarkScreen(2f));

        GameStateManager.Instance.JumpIntoFrogAndSlimeFight = true;
        //Set Jump into combat to be true
        ivesImage.gameObject.SetActive(false);
        gameOver.gameObject.SetActive(true);
        gameOver.FadeIn();

        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(gameLoseDialogue));
    }
}
