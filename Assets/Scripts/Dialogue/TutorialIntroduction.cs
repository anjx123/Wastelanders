using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialIntroduction : DialogueClasses
{
    [SerializeField] private Jackie jackie;
    [SerializeField] private Transform jackieDefaultTransform;
    [SerializeField] private EnemyIves ives;
    [SerializeField] private Transform ivesDefaultTransform;

    [SerializeField] private List<DialogueText> soldierGreeting;
    [SerializeField] private List<DialogueText> jackieTalksWithSolider;
    [SerializeField] private List<DialogueText> ivesScoldsJackie;

    private IEnumerator ExecuteGameStart()
    {
        CombatManager.Instance.GameState = GameState.OUT_OF_COMBAT;
        yield return new WaitForSeconds(3f);

        jackie.SetReturnPosition(jackieDefaultTransform.position);
        yield return StartCoroutine(jackie.ResetPosition()); //Jackie Runs into the scene

        yield return new WaitForSeconds(0.2f);

        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(soldierGreeting));

        yield return new WaitForSeconds(1f);
        jackie.FaceLeft(); //Jackie faces the soldier talking to her
        yield return new WaitForSeconds(0.2f);

        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(jackieTalksWithSolider));

        jackie.FaceRight(); //Jackie turns to face the person yelling at her

        yield return new WaitForSeconds(1f);

        ives.SetReturnPosition(ivesDefaultTransform.position);
        yield return StartCoroutine(ives.MoveToPosition(ivesDefaultTransform.position, 0, 0.8f)); //Ives comes into the scene

        yield return new WaitForSeconds(0.2f);

        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(ivesScoldsJackie));




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
