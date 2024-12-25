using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscreteStrike : StaffCards
{
    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 1;
        Speed = 1;

        myName = "Discrete Strike";
        description = "Gain 2 focus, then strike";
        CardType = CardType.MeleeAttack;
        base.Initialize();
    }

    public override void ApplyEffect()
    {
        Origin.AddStacks(Flow.buffName, 2);
        base.ApplyEffect();
    }
}
