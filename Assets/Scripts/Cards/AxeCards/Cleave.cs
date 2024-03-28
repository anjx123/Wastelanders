using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cleave : AxeCards
{
    public override void Initialize()
    {
        lowerBound = 3;
        upperBound = 5;
        Speed = 2;

        myName = "Cleave";
        description = "On hit, apply 2 stacks of wound.";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; 
        OriginalPosition = transform.position;
        base.Initialize();
    }

    public override void OnHit()
    {
        // something with stacks. 
        base.OnHit();
    }
}
