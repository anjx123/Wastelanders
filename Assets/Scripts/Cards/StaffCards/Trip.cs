using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class Trip : StaffCards
{
    public override void OnCardStagger()
    {
        Debug.Log("Executing Effect");
    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        CardType = CardType.MeleeAttack;
        myName = "Trip";
        description = "If this attack hits the opponent, gain 1 Focus";
        lowerBound = 2;
        upperBound = 4;
        Speed = 4;
        
        base.Initialize();
    }

    public override void OnHit()
    {
        base.OnHit();
        Origin.AddStacks(Focus.buffName, 1);
    }
}
