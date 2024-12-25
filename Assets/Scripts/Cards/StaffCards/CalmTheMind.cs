using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.UI.Image;

public class CalmTheMind : StaffCards
{

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 2;
        upperBound = 4;

        Speed = 5;
        
        myName = "Calm The Mind";
        description = "Block, then gain 2 stack of Flow. This card does not consume flow.";
        
        base.Initialize();
        CardType = CardType.Defense;
    }

    public override void ApplyEffect()
    {
        int stacks = Origin.GetBuffStacks(Flow.buffName);
        base.ApplyEffect();
        Origin.AddStacks(Flow.buffName, stacks);
    }

    public override void CardIsUnstaggered()
    {
        base.CardIsUnstaggered();
        Origin.AddStacks(Flow.buffName, 2);
    }

    public override void OnCardStagger()
    {
        base.OnCardStagger();
        Origin.AddStacks(Flow.buffName, 2);
    }
}
