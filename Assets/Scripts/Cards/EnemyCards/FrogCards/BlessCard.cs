using Random = UnityEngine.Random;

namespace Cards.EnemyCards.FrogCards
{
    public class BlessCard : ActionClass, IPlayablePrincessFrogCard
    {
        public const int BLESS_COST = 1;

        public override void Initialize()
        {
            base.Initialize();

            myName = "Bless";
            description =
                $"Spend +{BLESS_COST} Resonate to play. If not staggered: Gain 1 Resonate and give all teammates a random positive buff.";

            CostToAddToDeck = 2;
            lowerBound = upperBound = 1;
            Speed = 1;
            CardType = CardType.Defense;
        }

        public override void OnQueue()
        {
            Origin.ReduceStacks(Resonate.buffName, BLESS_COST);
        }

        public override void OnRetrieveFromQueue()
        {
            Origin.AddStacks(Resonate.buffName, BLESS_COST);
        }

        public override bool IsPlayableByPlayer(out PopupType popupType)
        {
            bool isPlayable = base.IsPlayableByPlayer(out popupType);
            bool enoughStacks = Origin.GetBuffStacks(Resonate.buffName) >= BLESS_COST;

            popupType = enoughStacks ? popupType : PopupType.InsufficientResources;

            return isPlayable && enoughStacks;
        }

        public override void CardIsUnstaggered()
        {
            Origin.AttackAnimation("IsBlocking");
            Origin.AddStacks(Resonate.buffName, BLESS_COST);

            var teamMates = Origin.Team.GetTeamMates();
            var buffs = new[] { Accuracy.buffName, Flow.buffName, Resonate.buffName };
            foreach (var enemy in teamMates)
            {
                enemy.AddStacks(buffs[Random.Range(0, buffs.Length)], 1);
            }
        }
    }
}