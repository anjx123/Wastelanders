using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.UI.Image;

public class CalmTheMind : StaffCards
{
    public override void OnCardStagger()
    {

        Origin.AddStacks(Focus.buffName, 1);
        base.OnCardStagger();
    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 2;
        upperBound = 3;

        Speed = 5;
        
        myName = "Calm The Mind";
        description = "Block, then gain 1 stack of focus.";
        
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
        base.Initialize();
        CardType = CardType.Defense;
    }

    public override void CardIsUnstaggered()
    {
        Origin.AddStacks(Focus.buffName, 2);
        base.CardIsUnstaggered();
    }
}
