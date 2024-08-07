using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cleave : AxeCards
{
    public override void Initialize()
    {
        lowerBound = 2;
        upperBound = 6;
        Speed = 2;

        myName = "Cleave";
        description = "On hit, apply 2 stacks of wound to the target.";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; 
        OriginalPosition = transform.position;
        base.Initialize();
        CardType = CardType.MeleeAttack;
    }

    public override void OnHit()
    {
        base.OnHit();
        Target.AddStacks(Wound.buffName, 2);
    }
}
