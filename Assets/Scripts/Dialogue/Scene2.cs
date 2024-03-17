using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene2 : DialogueClasses
{
    [SerializeField] private Jackie jackie;
    [SerializeField] private Transform jackieDefaultTransform;
    [SerializeField] private Transform jackieWander1;
    [SerializeField] private Transform jackieWander2;
    [SerializeField] private Transform jackieWander3;
    [SerializeField] private Transform outOfScreen;

    [SerializeField] private WasteFrog frog;
    [SerializeField] private Transform frogInitialWalkIn;

    [SerializeField] private List<DialogueText> sceneNarration;
    [SerializeField] private List<DialogueText> ivesInstruction;
    [SerializeField] private List<DialogueText> jackieStrategyPlan;
    // After the frog enters the scene
    [SerializeField] private List<DialogueText> jackieMissedShot;
    // After the frog is defeated
    [SerializeField] private List<DialogueText> crystalExtraction;

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
        jackie.OutOfCombat();
        frog.OutOfCombat();
        
        jackie.SetReturnPosition(jackieDefaultTransform.position);
        frog.SetReturnPosition(frogInitialWalkIn.position);

        frog.UnTargetable();
        if (!jumpToCombat)
        {
            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(sceneNarration));
            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(ivesInstruction));

            jackie.Emphasize();
            yield return StartCoroutine(jackie.ResetPosition());
            yield return StartCoroutine(CombatManager.Instance.FadeInLightScreen(MEDIUM_PAUSE));
            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(jackieStrategyPlan));
            yield return StartCoroutine(jackie.MoveToPosition(jackieWander1.position, 0f, 1.2f));
            yield return StartCoroutine(jackie.MoveToPosition(jackieWander2.position, 0f, 1.2f));
            jackie.FaceLeft();
            yield return StartCoroutine(jackie.MoveToPosition(jackieWander3.position, 0f, 1.2f));
            jackie.FaceRight();
            yield return StartCoroutine(jackie.MoveToPosition(jackieDefaultTransform.position, 0f, .5f));
            yield return StartCoroutine(WalkWhileScreenFades());
            // layering bush to be done l8r
        }
       
        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(jackieMissedShot));
        yield return StartCoroutine(jackie.ResetPosition());
        yield return StartCoroutine(frog.ResetPosition());
        yield return new WaitUntil(() => !DialogueManager.Instance.IsInDialogue());
        CombatManager.Instance.GameState = GameState.SELECTION;

        yield return new WaitUntil(() => CombatManager.Instance.GameState == GameState.GAME_WIN);

        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(crystalExtraction));
        yield return StartCoroutine(CombatManager.Instance.FadeInDarkScreen(2f));

        SceneManager.LoadScene("LevelSelect");

        yield break;
    }

    private IEnumerator WalkWhileScreenFades()
    {
        IEnumerator i1 = jackie.MoveToPosition(jackieWander2.position, 0f, 1.2f);
        IEnumerator i2 = CombatManager.Instance.FadeInDarkScreen(1f);
        StartCoroutine(i1);
        StartCoroutine(i2);
        while (i1.MoveNext() && i2.MoveNext()) { }
        yield break;
    }

}
