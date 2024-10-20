using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;
using Systems.Persistence;

public class PostQueenBeetle : DialogueClasses
{
    [SerializeField] private Jackie jackie;

    [SerializeField] private Transform jackieListensToBroadcast;
    [SerializeField] private Transform jackieIvesTalk;

    [SerializeField] private EnemyIves ives;

    [SerializeField] private CinemachineVirtualCamera ivesCamera;
    [SerializeField] private Transform mainCameraIvesTalk;

    [SerializeField] Sprite jackieSmileImage;
    [SerializeField] Image ivesImage;
    [SerializeField] Image jackieImage;


    [SerializeField] private List<DialogueText> jackieOpeningDialogue;


    [SerializeField] private List<DialogueText> soldierAndResults;
    [SerializeField] private List<DialogueText> resultsBroadcast;
    [SerializeField] private List<DialogueText> jackieResultsConfusion;
    [SerializeField] private List<DialogueText> ivesYapping;
    [SerializeField] private List<DialogueText> jackieInterjects;
    [SerializeField] private List<DialogueText> ivesFinal;
    [SerializeField] private List<DialogueText> ivesHug;
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
        ives.FaceLeft();
        yield return new WaitForSeconds(0.8f);

        jackie.OutOfCombat(); ives.OutOfCombat();

        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(jackieOpeningDialogue));
        yield return StartCoroutine(CombatManager.Instance.FadeInLightScreen(1f));
        yield return StartCoroutine(jackie.MoveToPosition(jackieListensToBroadcast.position, 0f, 1.2f));
        yield return new WaitForSeconds(1f);
        jackie.animator.enabled = false;
        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(soldierAndResults));
        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(resultsBroadcast));

        yield return new WaitForSeconds(0.8f);
        CombatManager.Instance.ActivateDynamicCamera();
        jackie.FaceLeft();
        yield return new WaitForSeconds(0.8f);
        jackie.animator.enabled = true;
        yield return StartCoroutine(jackie.MoveToPosition(jackieIvesTalk.position, 0f, 2f));
        yield return new WaitForSeconds(0.8f);
        jackie.animator.enabled = false;

        ivesCamera.transform.position = mainCameraIvesTalk.position;
        ivesCamera.Priority = 2;
        CombatManager.Instance.ActivateBaseCamera();
        yield return StartCoroutine(FadeImage(jackieImage, 1f, true));
        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(jackieResultsConfusion));
        yield return new WaitForSeconds(0.5f);

        ives.FaceRight(); ives.animator.enabled = false; yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(FadeImage(ivesImage, 1f, true));
        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(ivesYapping));
        yield return new WaitForSeconds(0.6f);
        jackieImage.sprite = jackieSmileImage;
        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(jackieInterjects));
        yield return new WaitForSeconds(0.6f);
        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(ivesFinal));
        yield return new WaitForSeconds(0.6f);

        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(ivesHug));
        Coroutine ivesFade = StartCoroutine(FadeImage(ivesImage, 1f, false));
        yield return StartCoroutine(FadeImage(jackieImage, 1f, false));
        yield return ivesFade;

        yield return StartCoroutine(CombatManager.Instance.FadeInDarkScreen(2f));

        GameStateManager.Instance.LoadScene(GameStateManager.CREDITS);    
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
}
