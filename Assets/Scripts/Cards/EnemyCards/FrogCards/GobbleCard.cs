using Entities;

namespace Cards.EnemyCards.FrogCards
{
    public class GobbleCard : ActionClass, IPlayablePrincessFrogCard
    {
        public override void Initialize()
        {
            base.Initialize();

            myName = "Gobble";
            description = "If attacking a crystal: Instantly destroy it.";

            lowerBound = upperBound = 1;
            CostToAddToDeck = 1;
            Speed = 3;
            CardType = CardType.MeleeAttack;
        }

        public override void CardIsUnstaggered()
        {
            Origin.AttackAnimation("IsShooting");
        }

        public override void OnHit()
        {
            AudioManager.Instance.PlaySFX(Excavate.EXCAVATE_SOUND_EFFECT_NAME);

            if (Target is Crystals) rolledCardStats.actualRoll = Target.Health;
            base.OnHit();
        }
    }
}