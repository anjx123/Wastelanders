using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Entities
{
    public class PrincessFrog : EnemyClass
    {
        [SerializeField] private GameObject[] spawnPrefabs;
        [SerializeField] private Vector3[] spawnPositions;

        private EntityClass[] _spawnSlots;

        public void Awake()
        {
            /* Each position is a slot for a spawn. If there are fewer prefabs than
               positions, then just cycle through the prefabs to fill the positions. */
            _spawnSlots = new EntityClass[spawnPositions.Length];

            for (var i = 0; i < _spawnSlots.Length; i++)
            {
                /* Start with fixed spawns. */
                SpawnNext(spawnPrefabs[i % spawnPrefabs.Length]);
            }
        }

        public override void Start()
        {
            base.Start();

            myName = "Princess Frog";
            Health = MaxHealth = 100;

            AddStacks(Resonate.buffName, 10);
        }

        public void OnEnable()
        {
            EntityTookDamage += HandleDamage;
            OnEntityDeath += HandleSpawnDeath;
        }

        public void OnDisable()
        {
            EntityTookDamage -= HandleDamage;
            OnEntityDeath -= HandleSpawnDeath;
        }

        public override void AddAttack(List<PlayerClass> players)
        {
            /* Required in this order, specifically... TODO: FIX THIS */
            var bless = pool[0];
            var burp = pool[1];
            var hurl = pool[2];
            var gobble = pool[3];

            // TODO: Add some actual AI to this.
            var playable = new List<GameObject>
            {
                bless,
                burp,
                hurl,
                gobble
            };

            for (var i = 0; i < 2; i++)
            {
                var prefab = playable[Random.Range(0, playable.Count)];
                var card = Instantiate(prefab);
                var action = card.GetComponent<ActionClass>();

                action.Target = players[Random.Range(0, players.Count)];
                action.Origin = this;
                BattleQueue.BattleQueueInstance.AddAction(action);
                combatInfo.AddCombatSprite(action);
            }
        }

        public void SpawnNext(GameObject prefab = null)
        {
            var slot = Array.IndexOf(_spawnSlots, null);
            if (slot == -1) return;

            // TODO: Maybe use the scene builder for this.
            var enemy = prefab ?? spawnPrefabs[Random.Range(0, spawnPrefabs.Length)];
            var p = transform.position + spawnPositions[slot];
            var spawn = Instantiate(enemy, p, Quaternion.identity, transform.parent);

            spawn.transform.localScale = Vector3.one * 0.75f;
            _spawnSlots[slot] = spawn.GetComponent<EnemyClass>();
        }

        private void HandleDamage(int amount)
        {
            if (amount == 0) return;

            /* Lose stacks when taking (non-zero) damage. */
            ReduceStacks(Resonate.buffName, 1);
        }

        private void HandleSpawnDeath(EntityClass spawn)
        {
            var slot = Array.IndexOf(_spawnSlots, spawn);
            if (slot == -1) return;

            _spawnSlots[slot] = null;
        }
    }
}