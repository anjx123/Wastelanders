using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Entities
{
    public class PrincessFrog : EnemyClass
    {
        public override void Start()
        {
            base.Start();

            myName = "Princess Frog";
            Health = MaxHealth = 100;
            AddStacks(Resonate.buffName, 10);
        }

        public void SetMaxHealth(int maxHealth)
        {
            Health = MaxHealth = maxHealth;
        }
        protected override void OnEnable()
        {
            base.OnEnable();

            EntityTookDamage += HandleDamage;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            EntityTookDamage -= HandleDamage;
        }

        public override void AddAttack(List<PlayerClass> players)
        {
            /* Required in this order, specifically... */
            var bless = deck[0];
            var burp = deck[1];
            var gobble = deck[2];
            var hurl = deck[3];

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