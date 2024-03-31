using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class Trip : StaffCards
{
    public override void OnCardStagger()
    {
        base.OnCardStagger();
        Origin.AddStacks(Flow.buffName, 1);
    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 2;
        upperBound = 4;

        Speed = 2;

        myName = "Sweep";
        description = "Attack, then gain 1 Flow.";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
        CardType = CardType.MeleeAttack;
        base.Initialize();
    }

    public override void CardIsUnstaggered()
    {
        base.CardIsUnstaggered();
        Origin.AddStacks(Flow.buffName, 1);
    }
}