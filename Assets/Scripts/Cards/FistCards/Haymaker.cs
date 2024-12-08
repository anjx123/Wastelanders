using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Haymaker : FistCards
{
    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 2;
        upperBound = 5;
        Speed = 5;

        myName = "Haymaker";
        description = "Deals a solid blow!";
        CardType = CardType.MeleeAttack;
        Renderer renderer = GetComponent<Renderer>();
        base.Initialize();
    }

}
