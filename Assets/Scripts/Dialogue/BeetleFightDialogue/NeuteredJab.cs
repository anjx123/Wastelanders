using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuteredJab : StaffCards
{
    // Start is called before the first frame update
    public override void Initialize()
    {
        CardType = CardType.MeleeAttack;
        myName = "Jab";
        description = "If this attack hits the opponent, gain 1 Focus";
        lowerBound = 2;
        upperBound = 4;
        Speed = 4;


        base.Initialize();
    }

}
