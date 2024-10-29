using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Entities
{
    public class PrincessFrog : EnemyClass
    {
        /* This enemy triggers changes in the scene layout by spawning enemies. */
        private SceneBuilder.SceneBuilder _sceneBuilder;

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
        }

        public void OnDisable()
        {
            EntityTookDamage -= HandleDamage;
        }

        public void SetSceneBuilder(SceneBuilder.SceneBuilder sceneBuilder)
        {
            _sceneBuilder = sceneBuilder;
        }

        public void OnBurp()
        {
            _sceneBuilder.SpawnAdditionalEnemy();
        }

        public override void AddAttack(List<PlayerClass> players)
        {
            /* Required in this order, specifically... */
            var bless = pool[0];
            var burp = pool[1];
            var gobble = pool[2];
            var hurl = pool[3];

            var enemyCount = CombatManager.Instance
                .GetEnemies()
                .Count(e => e is not NeutralEntityInterface) - 1;

            var crystals = CombatManager.Instance
                .GetEnemies()
                .Where(e => e is Crystals).ToArray();

            var stacks = GetBuffStacks(Resonate.buffName);
            var chance = enemyCount switch
            {
                4 => 1.0f,
                3 => 0.7f,
                2 => 0.5f,
                1 => 0.2f,
                _ => 0
            };

            switch (stacks)
            {
                case > 6:
                    AttackWith(Random.Range(0f, 1f) > chance ? burp : bless, players[Random.Range(0, players.Count)]);
                    AttackWith(Random.Range(0f, 1f) > chance ? burp : bless, players[Random.Range(0, players.Count)]);
                    break;
                case < 3 when crystals.Length > 0:
                    AttackWith(gobble, crystals[Random.Range(0, crystals.Length)]);
                    AttackWith(gobble, crystals[Random.Range(0, crystals.Length)]);
                    break;
                case < 3:
                    AttackWith(hurl, players[Random.Range(0, players.Count)]);
                    AttackWith(hurl, players[Random.Range(0, players.Count)]);
                    break;
                default:
                    if (crystals.Length > 0) AttackWith(gobble, crystals[Random.Range(0, crystals.Length)]);
                    else AttackWith(hurl, players[Random.Range(0, players.Count)]);
                    AttackWith(Random.Range(0f, 1f) > chance ? burp : bless, players[Random.Range(0, players.Count)]);
                    break;
            }
        }

        private void AttackWith(GameObject prefab, EntityClass target)
        {
            var card = Instantiate(prefab);
            var action = card.GetComponent<ActionClass>();

            action.Target = target;
            action.Origin = this;
            combatInfo.AddCombatSprite(action);
            BattleQueue.BattleQueueInstance.AddAction(action);
        }

        private void HandleDamage(int amount)
        {
            if (amount == 0) return;

            /* Lose stacks when taking (non-zero) damage. */
            ReduceStacks(Resonate.buffName, 1);
        }
    }
}