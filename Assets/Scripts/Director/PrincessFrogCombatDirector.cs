using System;
using System.Collections;
using LevelSelectInformation;
using UnityEngine;
using static BattleIntroEnum;

namespace Director
{
    public class PrincessFrogCombatDirector : MonoBehaviour
    {
        [SerializeField] private GameObject entityContainer;

        private BattleIntro battleIntro;
        [SerializeField] private GameOver gameOver;
        [SerializeField] private DialogueWrapper gameOverDialogue; // sucks...

        private void Start()
        {
            if (CombatManager.Instance.GameState != GameState.GAME_START) return;
            StartCoroutine(OnStart());
        }

        public void OnDisable()
        {
            CombatManager.PlayersWinEvent -= PlayersWin;
            CombatManager.EnemiesWinEvent -= EnemiesWin;
        }

        private IEnumerator OnStart()
        {
            CombatManager.Instance.SetDarkScreen();
            CombatManager.PlayersWinEvent += PlayersWin;
            CombatManager.EnemiesWinEvent += EnemiesWin;
            battleIntro = BattleIntro.Build(Camera.main);

            yield return new WaitForEndOfFrame(); // Necessary for associated initialization code to run (to assign teams)

            CombatManager.Instance.BeginCombat();
            yield return StartCoroutine(CombatManager.Instance.FadeInLightScreen(1.5f));
            battleIntro.PlayAnimation(Get<ClashIntro>());
            yield return new WaitUntil(() => CombatManager.Instance.GameState == GameState.GAME_WIN);
            AudioManager.Instance.FadeOutCurrentBackgroundTrack(2f);
            BountyManager.Instance.NotifyWin();
            GameStateManager.Instance.CurrentLevelProgress = GameStateManager.Instance.CurrentLevelProgress = Math.Max(GameStateManager.Instance.CurrentLevelProgress, StageInformation.PRINCESS_FROG_FIGHT.LevelID + 1f);
            yield return new WaitForSeconds(1f);
            yield return StartCoroutine(CombatManager.Instance.FadeInDarkScreen(1.5f));
            GameStateManager.Instance.LoadScene(SceneData.Get<SceneData.LevelSelect>().SceneName);
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