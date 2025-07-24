
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Steamworks {

    public class AchievementManager : PersistentSingleton<AchievementManager> {
#if STEAMWORKS_NET
        private int enemiesKilled = 0;

        // boolean flag to track whether one player has died in the current combat.
        // reset whenever a scene changes.
        private bool onePlayerDead = false;

        private PlayerClass[] players;

        protected override void Awake() {
            base.Awake(); // Handles singleton instance + DontDestroyOnLoad

            if (!SteamManager.Initialized) return;

            // Load current kill count from Steam stats on start
            SteamUserStats.GetStat("KILL_COUNT", out enemiesKilled);
            Debug.Log($"[AchievementManager] Loaded KILL_COUNT = {enemiesKilled}");

            // In case player already reached milestones before starting the game, sync achievements
            CheckKillAchievements();
            
            players = FindObjectsOfType<PlayerClass>();
        }

        private void OnEnable() {
            EntityClass.OnEntityDeath += HandleEntityDeath;
            SceneManager.activeSceneChanged += OnSceneChanged;
            CombatManager.PlayersWinEvent += OnPlayersWin;

            foreach (PlayerClass player in players) {
                player.BuffsUpdatedEvent += HandlePlayerBuffsUpdated;
            }
        }

        private void OnDisable() {
            EntityClass.OnEntityDeath -= HandleEntityDeath;
            SceneManager.activeSceneChanged -= OnSceneChanged;
            CombatManager.PlayersWinEvent -= OnPlayersWin;
            
            foreach (PlayerClass player in players) {
                player.BuffsUpdatedEvent -= HandlePlayerBuffsUpdated;
            }
        }

        private void HandleEntityDeath(EntityClass entity) {
            if (entity is EnemyClass) {
                // blacklist certain types of enemies here.
                if (entity is TrainingDummy) {
                    return;
                }
                
                if (entity is EnemyIves) {
                    SteamManager.UnlockAchievement("DEFEAT_IVES");
                }

                if (entity is QueenBeetle) {
                    SteamManager.UnlockAchievement("DEFEAT_QUEEN");
                }
                enemiesKilled++;
                Debug.Log($"[AchievementManager] Enemy killed! Total kills: {enemiesKilled}");

                // Update the persistent kill count stat on Steam
                SteamManager.UpdateStat("KILL_COUNT", enemiesKilled);

                // Check if any achievements should be unlocked based on new kill count
                CheckKillAchievements();
            } else if (entity is PlayerClass) {
                onePlayerDead = true;
            }
        }

        private void CheckKillAchievements() {
            if (enemiesKilled >= 1) {
                SteamManager.UnlockAchievement("FIRST_BLOOD");
            }

            if (enemiesKilled >= 5) {
                SteamManager.UnlockAchievement("WARMING_UP");
            }

            if (enemiesKilled >= 15) {
                SteamManager.UnlockAchievement("KILLING_SPREE");
            }

            // Add more milestones here
            // if (enemiesKilled >= 10) SteamManager.UnlockAchievement("HUNTER");
        }

        public void HandlePlayerHitCritical() {
            SteamManager.UnlockAchievement("CRITICAL");
        }

        private void OnSceneChanged(Scene arg0, Scene arg1) {
            onePlayerDead = false;
        }

        private void OnPlayersWin() {
            if (onePlayerDead) {
                SteamManager.UnlockAchievement("REVENGE");
            }
        }

        private void HandlePlayerBuffsUpdated(EntityClass player) {
            if (player is not PlayerClass) return;

            if (player.ResonateStacks >= 5) {
                SteamManager.UnlockAchievement("THE_RESONATOR");
            }
        }
        
#endif // STEAMWORKS_NET
    }
}