using Cards.EnemyCards.FrogCards;
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
            Health = MaxHealth = 75;
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
            // Required so retargeting one card does not retarget both.
            var bless2 = deck[4];
            var burp2 = deck[5];
            var gobble2 = deck[6];
            var hurl2 = deck[7];


            var enemyCount = CombatManager.Instance.GetEnemies().Count;

            var opponents = targets.Where(entity => entity.Team == EntityTeam.PlayerTeam).ToList();
            var neutral = targets.Where(entity => entity.Team == EntityTeam.NeutralTeam).ToList();

            var stacks = GetBuffStacks(Resonate.buffName);
            var chance = enemyCount switch
            {
                4 => 0.8f,
                3 => 0.5f,
                2 => 0.2f,
                1 => 0f,
                0 => 0f,
                _ => 1.0f
            };
            
            switch (stacks)
            {
                case > 6:
                    AttackWith(Random.Range(0f, 1f) > chance ? burp : bless, CalculateAttackTarget(opponents));
                    AttackWith(Random.Range(0f, 1f) > chance ? burp2 : bless2, CalculateAttackTarget(opponents));
                    break;
                case < 3 when neutral.Count > 0:
                    AttackWith(gobble, CalculateAttackTarget(neutral));
                    AttackWith(gobble2, CalculateAttackTarget(neutral));
                    break;
                case < 3:
                    AttackWith(hurl, CalculateAttackTarget(opponents));
                    AttackWith(hurl2, CalculateAttackTarget(opponents));
                    break;
                default: // 3 < case <= 6
                    if (neutral.Count > 0) AttackWith(gobble, CalculateAttackTarget(neutral));
                    else AttackWith(hurl, CalculateAttackTarget(opponents));

                    AttackWith(Random.Range(0f, 1f) > chance ? burp : bless, CalculateAttackTarget(opponents));
                    break;
            }
        }

       

        private void HandleDamage(int amount)
        {
            if (amount == 0) return;

            /* Lose stacks when taking (non-zero) damage. Commented out for balance for now*/
            //ReduceStacks(Resonate.buffName, 1);
        }
    }
}