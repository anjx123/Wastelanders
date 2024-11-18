using Random = UnityEngine.Random;

namespace Cards.EnemyCards.FrogCards
{
    public class BlessCard : FrogAttacks
    {
        public override void Initialize()
        {
            base.Initialize();

            myName = "Bless";
            description =
                "If not staggered: Gives all monsters a random positive buff, then this monster loses 1 Resonate.";

            lowerBound = upperBound = 1;
            Speed = 1;
            CardType = CardType.Defense;
        }

        public override void CardIsUnstaggered()
        {
            var stacks = Origin.GetBuffStacks(Resonate.buffName);
            if (stacks < 1) return;

            Origin.AttackAnimation("IsBlocking");
            Origin.ReduceStacks(Resonate.buffName, 1);

            var enemies = Origin.transform.parent.GetComponentsInChildren<EnemyClass>();
            var buffs = new[] { Accuracy.buffName, Flow.buffName, Resonate.buffName };
            foreach (var enemy in enemies)
            {
                enemy.AddStacks(buffs[Random.Range(0, buffs.Length)], 1);
            }
        }
    }
}