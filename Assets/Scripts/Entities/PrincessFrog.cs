using Cards.EnemyCards.FrogCards;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Entities
{
    public class PrincessFrog : EnemyClass
    {
        public int NumberOfAttacks { get; set; } = 2;
        public List<GameObject> BlessCards { get; private set; } = new();
        public List<GameObject> HurlCards { get; private set; } = new();
        public List<GameObject> BurpCards { get; private set; } = new();
        public List<GameObject> Spawnables { get; private set; } = new();
        public List<GameObject> GobbleCards { get; private set; } = new();
        public AttackDeciderDelegate AttackDecider { private get; set; } = 
            (int enemyCount) => 
                Random.Range(0f, 1f) > enemyCount switch
                {
                    4 => 1f,
                    3 => 0.66f,
                    2 => 0.33f,
                    1 => 0f,
                    0 => 0f,
                    _ => 1.0f
                };

        public delegate bool AttackDeciderDelegate(int opponentCount);

        public override void Start()
        {
            base.Start();

            myName = "Princess Frog";
            Health = MaxHealth = 75;
            AddStacks(Resonate.buffName, 8);
        }

        public override void InstantiateDeck()
        {
            var actionMapping = new Dictionary<int, List<GameObject>>
            {
                { 0, BlessCards },
                { 1, BurpCards },
                { 2, GobbleCards },
                { 3, HurlCards }
            };

            for (int i = 0; i < availableActions.Count; i++)
            {
                for (int j = 0; j < NumberOfAttacks; ++j)
                {
                    GameObject toAdd = Instantiate(availableActions[i]);
                    ActionClass addedClass = toAdd.GetComponent<ActionClass>();
                    addedClass.Origin = this;
                    if (addedClass is BurpCard burpCard)
                    {
                        burpCard.SerializedSpawnableEnemies.Clear();
                        burpCard.SerializedSpawnableEnemies.AddRange(Spawnables);
                    } 

                    if (actionMapping.TryGetValue(i, out var targetList))
                    {
                        targetList.Add(toAdd);
                    }
                }
            }

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
            var opponents = targets.Where(entity => entity.Team == EntityTeam.PlayerTeam).ToList();
            var neutral = targets.Where(entity => entity.Team == EntityTeam.NeutralTeam).ToList();

            // Variables that potentially change as the princess frog makes its attack sequentially
            int potentialEnemyCount = CombatManager.Instance.GetEnemies().Count; // Note that princess frog counts itself
            int gobblePotentialStacks = 0;

            for (int i = 0; i < NumberOfAttacks; i++)
            {
                bool shouldPlayBurp = AttackDecider(potentialEnemyCount);
                int currentStacks = GetBuffStacks(Resonate.buffName);

                switch (currentStacks)
                {
                    case >= 7:
                        AttackWith(shouldPlayBurp ? BurpCards[i] : BlessCards[i], CalculateAttackTarget(opponents));
                        if (shouldPlayBurp) ++potentialEnemyCount; // Increase potential enemy count so we do not over spawn.
                        break;
                    case var _ when neutral.Count > 0 && (gobblePotentialStacks + currentStacks) <= 6:
                        AttackWith(GobbleCards[i], CalculateAttackTarget(neutral));
                        gobblePotentialStacks += 3; //Pretends gobble succeeds and makes furthur decisions from there. 
                        break;
                    case >= 2 and <= 6:
                        AttackWith(shouldPlayBurp ? BurpCards[i] : BlessCards[i], CalculateAttackTarget(opponents));
                        if (shouldPlayBurp) ++potentialEnemyCount;
                        break;
                    case 1:
                        AttackWith(BlessCards[i], CalculateAttackTarget(neutral));
                        break;
                    case 0:
                        AttackWith(HurlCards[i], CalculateAttackTarget(neutral));
                        break;
                    default:
                        AttackWith(HurlCards[i], CalculateAttackTarget(neutral));
                        break;
                }

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