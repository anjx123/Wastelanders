using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialIntroduction : DialogueClasses
{
    [SerializeField] private Jackie jackie;
    [SerializeField] private Transform jackieDefaultTransform;
    [SerializeField] private List<DialogueText> dialogueText;

    private IEnumerator ExecuteGameStart()
    {
        CombatManager.Instance.GameState = GameState.OUT_OF_COMBAT;
        yield return new WaitForSeconds(3f);

        jackie.SetReturnPosition(jackieDefaultTransform.position);
        yield return StartCoroutine(jackie.ResetPosition());

        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(dialogueText));

        yield return new WaitForSeconds(1f);
        jackie.FaceLeft();



        yield break;
    }

    protected override void GameStateChange(GameState gameState)
    {
        if (gameState == GameState.GAME_START)
        {
            StartCoroutine(ExecuteGameStart());
        }
    }
}
