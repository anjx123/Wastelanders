using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrincessHurl : FrogAttacks, IPlayablePrincessFrogCard
{
    // Start is called before the first frame update
    public override void Initialize()
    {
        base.Initialize();
        lowerBound = 3;
        upperBound = 7;

        Speed = 5;

        CostToAddToDeck = 2;

        myName = "PrincessHurl";
        description = "Watch out!";
    }
}

