using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Haymaker : FistCards
{
    public override void ExecuteActionEffect()
    {

    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 2;
        upperBound = 5;
        Speed = 1;

        myName = "Haymaker";
        description = "Deals A Solid Blow";
        
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
        base.Initialize();
    }

    public override void OnHit()
    {
        base.OnHit();
    }
}