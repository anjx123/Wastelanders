using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class Trip : StaffCards
{
    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 2;
        upperBound = 4;

        Speed = 2;

        myName = "Sweep";
        description = "Attack, this card does not consume flow.";
        CardType = CardType.MeleeAttack;
        base.Initialize();
    }

    public override void ApplyEffect()
    {
        int stacks = Origin.GetBuffStacks(Flow.buffName);
        base.ApplyEffect();
        Origin.AddStacks(Flow.buffName, stacks);
    }
}