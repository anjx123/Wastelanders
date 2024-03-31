using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vitals : PistolCards
{
    public override void OnCardStagger()
    {

    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 2;
        upperBound = 7;

        Speed = 5;

        myName = "Vitals";
        description = "Hit 'em where it hurts.";
        CardType = CardType.RangedAttack;
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
        base.Initialize();


    }

}
