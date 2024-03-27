using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Execute : AxeCards
{
    public override void Initialize()
    {
        lowerBound = 2;
        upperBound = 6;
        Speed = 1;

        myName = "Execute";
        description = "On hit, deal an extra +1 damage for each wound on the enemy.";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material;
        OriginalPosition = transform.position;
        base.Initialize();
    }
}
