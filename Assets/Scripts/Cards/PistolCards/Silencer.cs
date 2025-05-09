using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Silencer : PistolCards
{

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 4;
        Speed = 3;
        description = "On hit, gain 2 Accuracy stacks.";
        myName = "Silencer";
        CardType = CardType.RangedAttack;
        base.Initialize();
    }

    public override void OnHit()
    {
        base.OnHit();
        Origin.AddStacks(Accuracy.buffName, 2);

    }
}
