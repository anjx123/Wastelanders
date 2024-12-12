using Random = UnityEngine.Random;

namespace Cards.EnemyCards.FrogCards
{
    public class BlessCard : ActionClass, IPlayablePrincessFrogCard
    {
        public override void Initialize()
        {
            base.Initialize();

            myName = "Bless";
            description =
                "If not staggered: Gives all teammates a random positive buff, then I lose 1 Resonate.";

            CostToAddToDeck = 2;
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

            var teamMates = Origin.Team.GetTeamMates();
            var buffs = new[] { Accuracy.buffName, Flow.buffName, Resonate.buffName };
            foreach (var enemy in teamMates)
            {
                enemy.AddStacks(buffs[Random.Range(0, buffs.Length)], 1);
            }
        }
    }
}