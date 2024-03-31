using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jab : StaffCards
{
    // Start is called before the first frame update
    public override void Initialize()
    {
        CardType = CardType.MeleeAttack;
        myName = "Jab";
        description = "On hit, gain 2 Flow.";
        lowerBound = 2;
        upperBound = 4;
        Speed = 4;

        base.Initialize();
    }

    public override void OnHit()
    {
        base.OnHit();
        Origin.AddStacks(Flow.buffName, 2);
    }
}
