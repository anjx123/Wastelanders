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

            AddStacks(Resonate.buffName, 25);
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

            var playable = new List<GameObject>();
            const int attacks = 2;

            /* If any spawn is alive, "Bless" is playable. If this has 2 or more
               stacks of Resonate, "Burp" is playable. "Hurl" is always playable. */
            if (Array.Exists(_spawnSlots, it => it)) playable.Add(bless);
            if (GetBuffStacks(Resonate.buffName) >= 2) playable.Add(burp);
            if (playable.Count == 0) playable.Add(hurl);

            for (var i = 0; i < attacks; i++)
            {
                var prefab = playable[Random.Range(0, playable.Count)];
                var card = Instantiate(prefab);
                var action = card.GetComponent<ActionClass>();

                action.Target = players[Random.Range(0, players.Count)];
                action.Origin = this;
                BattleQueue.BattleQueueInstance.AddAction(action);
                combatInfo.AddCombatSprite(action);

                // TODO: Move this to CardIsUnstaggered of Burp
                if (prefab == burp) ReduceStacks(Resonate.buffName, 2);
            }
        }

        public void SpawnNext(GameObject prefab)
        {
            var slot = Array.IndexOf(_spawnSlots, null);
            if (slot == -1) return;
            var spawn = Instantiate(prefab, spawnPositions[slot], Quaternion.identity, transform.parent);

            spawn.transform.localScale *= 0.5f;
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