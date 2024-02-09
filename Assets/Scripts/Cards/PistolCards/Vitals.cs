using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vitals : PistolCards
{
    public override void ExecuteActionEffect()
    {

    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 8;

        Speed = 3;
        Block = 2;

        myName = "Vitals";
        description = "Placeholder";
        CardType = CardType.RangedAttack;
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
        base.Initialize();


    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void ApplyEffect()
    {

        base.ApplyEffect();

    }
}
