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

        public override void AddAttack(List<EntityClass> targets)
        {
            /* Required in this order, specifically... */
            var bless = deck[0];
            var burp = deck[1];
            var gobble = deck[2];
            var hurl = deck[3];

            var enemyCount = CombatManager.Instance.GetEnemies().Count;

            var crystals = CombatManager.Instance.GetNeutral();

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
                    AttackWith(Random.Range(0f, 1f) > chance ? burp : bless, AttackTargetCalculator(targets));
                    AttackWith(Random.Range(0f, 1f) > chance ? burp : bless, AttackTargetCalculator(targets));
                    break;
                case < 3 when crystals.Count > 0:
                    AttackWith(gobble, AttackTargetCalculator(crystals));
                    AttackWith(gobble, AttackTargetCalculator(crystals));
                    break;
                case < 3:
                    AttackWith(hurl, AttackTargetCalculator(targets));
                    AttackWith(hurl, AttackTargetCalculator(targets));
                    break;
                default:
                    if (crystals.Count > 0) AttackWith(gobble, AttackTargetCalculator(crystals));
                    else AttackWith(hurl, AttackTargetCalculator(targets));

                    AttackWith(Random.Range(0f, 1f) > chance ? burp : bless, AttackTargetCalculator(targets));
                    break;
            }
        }

       

        private void HandleDamage(int amount)
        {
            if (amount == 0) return;

            /* Lose stacks when taking (non-zero) damage. */
            ReduceStacks(Resonate.buffName, 1);
        }
    }
}