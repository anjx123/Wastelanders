using Entities;
using UnityEngine;

namespace Cards.EnemyCards.FrogCards
{
    public class BurpCard : FrogAttacks
    {
        public override void Initialize()
        {
            base.Initialize();

            myName = "Burp";
            description = "If not staggered: Spawns a random monster, then this monster loses 2 Resonate.";

            lowerBound = upperBound = 1;
            Speed = 2;
            CardType = CardType.Defense;

            ogMaterial = GetComponent<Renderer>().material;
            OriginalPosition = transform.position;
        }

        public override void CardIsUnstaggered()
        {
            var stacks = Origin.GetBuffStacks(Resonate.buffName);
            if (stacks < 2) return;

            Origin.AttackAnimation("OnSmile");
            Origin.ReduceStacks(Resonate.buffName, 2);

            var princessFrog = Origin as PrincessFrog;
            princessFrog?.OnBurp(); // TODO: Any way to do this without casting?
        }
    }
}