using Entities;
using UnityEngine;

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

            ogMaterial = GetComponent<Renderer>().material;
            OriginalPosition = transform.position;
        }

        public override void CardIsUnstaggered()
        {
            Origin.AttackAnimation("OnSpit");
        }

        public override void OnHit()
        {
            base.OnHit();

            var crystal = Target as Crystals;
            if (!crystal) return;

            var prior = Origin.GetBuffStacks(Resonate.buffName);
            crystal.TakeDamage(Origin, crystal.Health);
            /* Silly hack to always grant 5 stacks. */
            Origin.AddStacks(Resonate.buffName, 5 + prior - Origin.GetBuffStacks(Resonate.buffName));
        }
    }
}