using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheapStrike : StaffCards
{
    public override void OnCardStagger()
    {

    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 1;

        Speed = 1;


        myName = "Cheap Strike";
        description = "On hit, gain 3 Flow.";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
        CardType = CardType.MeleeAttack;
        base.Initialize();


    }

    public override void OnHit()
    {
        base.OnHit();
        Origin.AddStacks(Flow.buffName, 3);
    }
}
