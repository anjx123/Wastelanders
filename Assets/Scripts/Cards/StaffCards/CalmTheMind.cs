using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.UI.Image;

public class CalmTheMind : StaffCards
{
    int stacksConsumed = 0;
    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 2;
        upperBound = 4;

        Speed = 5;
        
        myName = "Calm The Mind";
        description = "Block, then gain Flow equal to 2 + Flow consumed.";
        
        base.Initialize();
        CardType = CardType.Defense;
    }

    public override void ApplyEffect()
    {
        stacksConsumed = Origin.GetBuffStacks(Flow.buffName);
        base.ApplyEffect();
    }

    public override void CardIsUnstaggered()
    {
        base.CardIsUnstaggered();
        Origin.AddStacks(Flow.buffName, 2 + stacksConsumed);
        stacksConsumed = 0;
    }

    public override void OnCardStagger()
    {
        base.OnCardStagger();
        Origin.AddStacks(Flow.buffName, 2 + stacksConsumed);
        stacksConsumed = 0;
    }
}
