using System.Collections;
using UnityEngine;

namespace Director
{
    public class PrincessFrogCombatDirector : MonoBehaviour
    {
        [SerializeField] private GameObject entityContainer;

        [SerializeField] private GameOver gameOver;
        [SerializeField] private DialogueWrapper gameOverDialogue; // sucks...

        private void Start()
        {
            if (CombatManager.Instance.GameState != GameState.GAME_START) return;
            StartCoroutine(OnStart());
        }

        private IEnumerator OnStart()
        {
            yield return StartCoroutine(CombatManager.Instance.FadeInLightScreen(0.5f));

            var entities = entityContainer.GetComponentsInChildren<EntityClass>();
            foreach (var entity in entities)
            {
                entity.InCombat();
                entity.SetReturnPosition(entity.transform.position);
            }

            CombatManager.Instance.GameState = GameState.SELECTION;
            CombatManager.PlayersWinEvent += PlayersWin;
            CombatManager.EnemiesWinEvent += EnemiesWin;

            yield return new WaitUntil(() => CombatManager.Instance.GameState == GameState.GAME_WIN);
            MusicManager.Instance.FadeOutCurrentBackgroundTrack(2f);
            yield return new WaitForSeconds(1f);
            yield return StartCoroutine(CombatManager.Instance.FadeInDarkScreen(1.5f));
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
            gameOver.gameObject.SetActive(true);
            gameOver.FadeIn();
            yield return StartCoroutine(DialogueManager.Instance.StartDialogue(gameOverDialogue.Dialogue));
        }
    }
}