using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialIntroduction : DialogueClasses
{
    [SerializeField] private Jackie jackie;
    [SerializeField] private Transform jackieDefaultTransform;
    [SerializeField] private EnemyIves ives;
    [SerializeField] private Transform ivesDefaultTransform;
    [SerializeField] private GameObject trainingDummyPrefab;
    [SerializeField] private Transform dummy1StartingPos;

    [SerializeField] private List<DialogueText> soldierGreeting;
    [SerializeField] private List<DialogueText> jackieTalksWithSolider;
    [SerializeField] private List<DialogueText> ivesScoldsJackie;

    private IEnumerator ExecuteGameStart()
    {
        CombatManager.Instance.GameState = GameState.OUT_OF_COMBAT;
        yield return new WaitForSeconds(3f);
        ives.OutOfCombat();
        jackie.OutOfCombat(); //Workaround for now, ill have to remove this once i manually start instantiating players
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

        yield return StartCoroutine(ives.MoveToPosition(dummy1StartingPos.position, 1.2f, 1.2f)); //Ives goes to place a dummy down
        Instantiate(trainingDummyPrefab, dummy1StartingPos);


        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(jackie.MoveToPosition(dummy1StartingPos.position, 1.4f, 0.8f));


        yield return new WaitForSeconds(1f);
        ives.InCombat();
        jackie.InCombat(); //Workaround for now, ill have to remove this once i manually start instantiating players
        CombatManager.Instance.GameState = GameState.SELECTION;







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
