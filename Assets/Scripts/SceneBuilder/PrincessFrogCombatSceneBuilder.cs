using System.Collections;
using System.Linq;
using Cards.EnemyCards.FrogCards;
using UnityEngine;

namespace SceneBuilder
{
    public class PrincessFrogCombatSceneBuilder : SceneBuilder
    {
        [SerializeField] private Vector3 playersPosition;
        [SerializeField] private Vector3 enemiesPosition;

        [SerializeField] private GameObject[] players;
        [SerializeField] private GameObject[] enemies;
        [SerializeField] private GameObject[] additionalEnemies;

        [SerializeField] private GameObject entityContainer;

        protected override void Build()
        {
            // TODO: Check for various challenges.

            // TODO: Un-fuck this.
            BurpCard.SpawnableEnemies.AddRange(additionalEnemies);

            SpawnAll(players, playersPosition);
            SpawnAll(enemies, enemiesPosition);

            EntityClass.OnEntitySpawn += HandleEntityChange;
            EntityClass.OnEntityDeath += HandleEntityChange;
        }

        private void SpawnAll(GameObject[] prefabs, Vector3 position)
        {
            var positions = PositionsFrom(position, prefabs.Length);
            for (var i = 0; i < prefabs.Length; i++)
            {
                Spawn(prefabs[i], positions[i]);
            }
        }

        private void Spawn(GameObject prefab, Vector3 position)
        {
            var spawn = Instantiate(prefab, position, Quaternion.identity, entityContainer.transform);
            var entity = spawn.GetComponent<EntityClass>();

            entity.transform.localScale = Vector3.one * (entity is Beetle ? 0.75f : 1);
        }

        private IEnumerator UpdateEnemyLayout()
        {
            /* The combat manager enemy list is at least one frame behind.
               This also fixes an issue where a newly spawned enemy would
               set return position in Start(), clobbering the given value. */
            yield return null;

            /* The combat manager doesn't remove enemies from its list until the end of the
               current turn. This method can be called during a turn which means we need to
               explicitly filter out any enemies who are dead but have not been removed yet. */
            var spawns = CombatManager.Instance
                .GetEnemies()
                .Where(e => e is not NeutralEntityInterface && !e.IsDead).ToArray();

            var positions = PositionsFrom(enemiesPosition, spawns.Length);
            for (var i = 0; i < spawns.Length; i++)
            {
                spawns[i].SetReturnPosition(entityContainer.transform.position + positions[i]);
            }
        }

        private void HandleEntityChange(EntityClass entity)
        {
            if (entity is not EnemyClass) return;

            StartCoroutine(UpdateEnemyLayout());
        }

        private static Vector3[] PositionsFrom(Vector2 centerCoordinate, int count)
        {
            var dx = -1f * Mathf.Sign(centerCoordinate.x);
            const float dy = 1f;

            var height = (count - 1) * dy;
            var top = centerCoordinate.y + height / 2f;

            var positions = new Vector3[count];
            for (var i = 0; i < count; i++)
            {
                positions[i] = new Vector3(centerCoordinate.x + (i % 2 == 0 ? 1f : -1f) * dx, top - i * dy);
            }

            return positions;
        }
    }
}