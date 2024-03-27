using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.UI.Image;

public class FocusedDefense : StaffCards
{
    public override void OnCardStagger()
    {

    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 2;
        upperBound = 4;

        Speed = 5;


        myName = "Focused Defense";
        description = "Gain 1 Focus, If this card is unstaggerd, Gain another Focus";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
        base.Initialize();
        CardType = CardType.Defense;
    }

    public override void CardIsUnstaggered()
    {
        Origin.AddStacks(Focus.buffName, 1);
    }
    public override void ApplyEffect()
    {
        Origin.AddStacks(Focus.buffName, 1);
    }
}
