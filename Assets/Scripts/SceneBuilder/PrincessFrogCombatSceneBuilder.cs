using System.Linq;
using UnityEngine;

namespace SceneBuilder
{
    public class PrincessFrogCombatSceneBuilder : SceneBuilder
    {
        [SerializeField] private GameObject[] princessFrog;
        [SerializeField] private GameObject[] players;
        [SerializeField] private Vector3 princessFrogPosition;
        [SerializeField] private Vector3 playersPosition;

        [SerializeField] private GameObject entityContainer;

        private GameObject _frog;

        private void Start()
        {
            /* Can't happen in Awake() due to an initialization order fiasco. */
            _frog?.GetComponent<EntityClass>().FaceLeft();
        }

        protected override void Build()
        {
            // TODO: Check for various challenges.

            SpawnEnemies();
            SpawnPlayers();
        }

        private void SpawnEnemies()
        {
            if (princessFrog.Length == 0) return;
            _frog = Spawn(princessFrog.First(), princessFrogPosition);
        }

        private void SpawnPlayers()
        {
            var positions = PositionsFrom(playersPosition, players.Length);
            for (var i = 0; i < players.Length; i++)
            {
                Spawn(players[i], positions[i]);
            }
        }

        private GameObject Spawn(GameObject entity, Vector3 position)
        {
            return Instantiate(entity, position, Quaternion.identity, entityContainer.transform);
        }

        private static Vector3[] PositionsFrom(Vector3 center, int count)
        {
            var dx = -1.0f * Mathf.Sign(center.x);
            const float dy = 1.5f;

            float n = count - 1;

            var offset = new Vector3(dx * n, dy * n, 0) / 2;
            var to = center + offset;
            var from = center - offset;

            return Enumerable.Range(0, count).Select(i => Vector3.Lerp(from, to, i / n)).ToArray();
        }
    }
}