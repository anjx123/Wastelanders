using System;
using System.Collections.Generic;
using System.Linq;
using BountySystem;
using Cards.EnemyCards.FrogCards;
using Entities;
using UnityEngine;

namespace SceneBuilder
{
    public class PrincessFrogCombatSceneBuilder : SceneBuilder
    {
        [SerializeField] private Vector3 playersPosition;
        [SerializeField] private Vector3 enemiesPosition;

        [SerializeField] private Jackie jackiePrefab;
        [SerializeField] private Ives ivesPrefab;

        [SerializeField] private PrincessFrog princessFrogPrefab;
        [SerializeField] private QueenBeetle queenBeetlePrefab;
        [SerializeField] private Beetle[] beetlePrefabs;
        [SerializeField] private WasteFrog frogPrefab;
        [SerializeField] private SlimeStack slimePrefab;

        [SerializeField] private GameObject entityContainer;

#nullable enable
        private IBounties? bounty = null;

        protected override void Build()
        {
            bounty = BountyManager.Instance.ActiveBounty;
            AdjustBurpCard(); 

            SpawnAll(DeterminePlayers(), playersPosition);
            SpawnAll(DetermineEnemies(), enemiesPosition);
        }

        public void OnEnable()
        {
            EntityClass.OnEntitySpawn += HandleEntityChange;
            EntityClass.OnEntityDeath += HandleEntityChange;
        }

        public void OnDisable()
        {
            EntityClass.OnEntitySpawn -= HandleEntityChange;
            EntityClass.OnEntityDeath -= HandleEntityChange;
        }

        private void AdjustBurpCard()
        {
            BurpCard.SpawnableEnemies.Clear();
            BurpCard.SpawnableEnemies.AddRange(DetermineBurpSpawnable());
            //TODO: Adjust the AI targeting for enemies spawened by burp card here too 
        }

        private IEnumerable<GameObject> DetermineBurpSpawnable()
        {
            List<GameObject> list = new();

            if (bounty?.ContractSet.Contains(EnemySpawningContracts.FROG_SPAWN) == true)
            {
                list.Add(frogPrefab.gameObject);
            } 
            else if (bounty?.ContractSet.Contains(EnemySpawningContracts.SLIME_SPAWN) == true)
            {
                list.Add(slimePrefab.gameObject);
            }
            else
            {
                list.AddRange(beetlePrefabs.Select(beetle => beetle.gameObject));
            }

            return list;
        }

        private GameObject[] DeterminePlayers()
        {
            List<GameObject> list = new();

            if (bounty?.ContractSet.Contains(PlayerContracts.SOLO_JACKIE) == true)
            {
                list.Add(jackiePrefab.gameObject);
            } 
            else
            {
                list.Add(jackiePrefab.gameObject);
                list.Add(ivesPrefab.gameObject);
            }

            return list.ToArray();
        }

        private GameObject[] DetermineEnemies()
        {
            List<GameObject> list = new();

            if (bounty?.ContractSet.Contains(EnemySpawningContracts.FROG_SPAWN) == true)
            {
                list.Add(frogPrefab.gameObject);
                list.Add(frogPrefab.gameObject);
                list.Add(frogPrefab.gameObject);
            }
            else if (bounty?.ContractSet.Contains(EnemySpawningContracts.SLIME_SPAWN) == true)
            {
                list.Add(slimePrefab.gameObject);
                list.Add(slimePrefab.gameObject);
                list.Add(slimePrefab.gameObject);
            }
            else if (bounty?.ContractSet.Contains(EnemySpawningContracts.QUEEN_BEETLE_SPAWN) == true)
            {
                list.Add(queenBeetlePrefab.gameObject);
            }
            else
            {
                list.AddRange(beetlePrefabs.Select(beetle => beetle.gameObject));
            }

            list.Add(princessFrogPrefab.gameObject);

            return list.ToArray();
        }

        private void AdjustPrincessFrog(PrincessFrog princessFrog)
        {
            if (bounty == null) return;

            // TODO: Implement Swappable AI
            if (bounty.ContractSet.Contains(PrincessFrogContracts.ADDITIONAL_ATTACK))
            {

            }

            if (bounty.ContractSet.Contains(PrincessFrogContracts.AGGRESIVE_AI))
            {

            }

            if (bounty.ContractSet.Contains(PrincessFrogContracts.EXTRA_HP))
            {
                princessFrog.SetMaxHealth(150);
            }

            if (bounty.ContractSet.Contains(PrincessFrogContracts.HIGHER_ENEMIES_CAP))
            {

            }
        }

        private void AdjustPlayerClass(PlayerClass playerClass)
        {
            if (bounty?.ContractSet.Contains(PlayerContracts.DECREASED_HAND_SIZE) == true)
            {
                // Implement Decreasable hand size
            }
        }

        private void AdjustEnemyClass(EnemyClass enemyClass)
        {
            // TODO: Adjust enemy targeting AI here
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
            
            if (entity is PrincessFrog princessFrog)
            {
                AdjustPrincessFrog(princessFrog);
            }
            else if (entity is EnemyClass enemyClass)
            {
                AdjustEnemyClass(enemyClass);
            }
            else if (entity is PlayerClass playerClass)
            {
                AdjustPlayerClass(playerClass);
            }
        }

        private void UpdateEnemyLayout()
        {
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

            UpdateEnemyLayout();
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