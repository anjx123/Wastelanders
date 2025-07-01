using UnityEngine;

namespace Steamworks {

    public class AchievementManager : MonoBehaviour {
        private int enemiesKilled = 0;

        private void OnEnable() {
            EntityClass.OnEntityDeath += HandleEntityDeath;
        }

        private void OnDisable() {
            EntityClass.OnEntityDeath -= HandleEntityDeath;
        }

        private void Start() {
            if (!SteamManager.Initialized) return;

            // Load current kill count from Steam stats on start
            SteamUserStats.GetStat("KILL_COUNT", out enemiesKilled);
            Debug.Log($"[AchievementManager] Loaded KILL_COUNT = {enemiesKilled}");

            // In case player already reached milestones before starting the game, sync achievements
            CheckKillAchievements();
        }

        private void HandleEntityDeath(EntityClass entity) {
            if (entity is EnemyClass) {
                enemiesKilled++;
                Debug.Log($"[AchievementManager] Enemy killed! Total kills: {enemiesKilled}");

                // Update the persistent kill count stat on Steam
                SteamManager.UpdateStat("KILL_COUNT", enemiesKilled);

                // Check if any achievements should be unlocked based on new kill count
                CheckKillAchievements();
            }
        }

        private void CheckKillAchievements() {
            if (enemiesKilled >= 1) {
                SteamManager.UnlockAchievement("FIRST_BLOOD");
            }

            if (enemiesKilled >= 5) {
                SteamManager.UnlockAchievement("WARMING_UP");
            }

            // Add more milestones here, e.g.
            // if (enemiesKilled >= 10) SteamManager.UnlockAchievement("HUNTER");
        }
    }
}