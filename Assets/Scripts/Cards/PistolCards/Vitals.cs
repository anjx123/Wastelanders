using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vitals : PistolCards
{
    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 7;

        Speed = 5;

        myName = "Vitals";
        description = "Hit 'em where it hurts.";
        CardType = CardType.RangedAttack;
        Renderer renderer = GetComponent<Renderer>();
        base.Initialize();
    }

}
