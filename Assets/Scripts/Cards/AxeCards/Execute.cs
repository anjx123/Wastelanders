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

    
    public override void OnHit()
    {
        // now this is weird: you will have to modify it quite a bit, because you're basing your attack on the Wound the enemy has i.e. your
        // duplicate card doesn't know whether or not the enemy is wounded. 
        base.OnHit();
    }
}
