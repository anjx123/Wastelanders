using Entities;

namespace Cards.EnemyCards.FrogCards
{
    public class GobbleCard : FrogAttacks
    {
        public override void Initialize()
        {
            base.Initialize();

            myName = "Gobble";
            description = "If attacking a crystal: Instantly destroys it, then this monster gains 5 Resonate.";

            lowerBound = upperBound = 2;
            Speed = 3;
            CardType = CardType.MeleeAttack;
        }

        public override void CardIsUnstaggered()
        {
            Origin.AttackAnimation("IsShooting");
        }

        public override void OnHit()
        {
            base.OnHit();

            var crystal = Target as Crystals;
            if (crystal)
            {
                var prior = Origin.GetBuffStacks(Resonate.buffName);
                crystal.TakeDamage(Origin, crystal.Health);
                /* Silly hack to always grant 5 stacks. */
                Origin.AddStacks(Resonate.buffName, 5 + prior - Origin.GetBuffStacks(Resonate.buffName));
            }
            /* FrogAttacks doesn't call this in OnHit except via a projectile...? */
            else Target.TakeDamage(Origin, rolledCardStats.actualRoll);
        }
    }
}