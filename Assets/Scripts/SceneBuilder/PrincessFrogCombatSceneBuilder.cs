using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entities;
using UnityEngine;
using Random = UnityEngine.Random;

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

        public override void SpawnAdditionalEnemy()
        {
            Spawn(additionalEnemies[Random.Range(0, additionalEnemies.Length)], enemiesPosition);
            StartCoroutine(UpdateEnemyLayout());
        }

        protected override void Build()
        {
            // TODO: Check for various challenges.

            SpawnAll(players, playersPosition);
            SpawnAll(enemies, enemiesPosition);

            EntityClass.OnEntityDeath += HandleEntityDeath;
            entityContainer.GetComponentInChildren<PrincessFrog>().SetSceneBuilder(this);
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
            entity.InCombat();
            entity.SetReturnPosition(entity.transform.position);
        }

        private IEnumerator UpdateEnemyLayout()
        {
            /* The combat manager enemy list is at least one frame behind.
               This also fixes an issue where a newly spawned enemy would
               set return position in Start(), clobbering the given value.*/
            yield return null;
            
            var spawns = CombatManager.Instance
                .GetEnemies()
                .Where(e => e is not NeutralEntityInterface).ToArray();

            var positions = PositionsFrom(enemiesPosition, spawns.Length);
            for (var i = 0; i < spawns.Length; i++)
            {
                spawns[i].SetReturnPosition(entityContainer.transform.position + positions[i]);
            }
        }

        private void HandleEntityDeath(EntityClass entity)
        {
            if (entity is EnemyClass) StartCoroutine(UpdateEnemyLayout());
        }

        private static Vector3[] PositionsFrom(Vector3 center, int count)
        {
            var dx = -1f * Mathf.Sign(center.x);
            var dy = center.x == 0 ? 0 : 1f;

            float n = count - 1;

            var offset = new Vector3(dx * n, dy * n, 0) / 2;
            var to = center + offset;
            var from = center - offset;

            return Enumerable.Range(0, count).Select(i => Vector3.Lerp(from, to, n < 1 ? 0 : i / n)).ToArray();
        }
    }
}