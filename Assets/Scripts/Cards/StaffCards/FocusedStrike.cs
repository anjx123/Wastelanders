using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusedStrike : StaffCards
{
    public override void OnCardStagger()
    {
        Debug.Log("Executing Effect");
    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        CardType = CardType.MeleeAttack;
        myName = "Focused Strike";
        description = "Double your Flow then make this attack. If unstaggered, gain 1 Flow.";
        lowerBound = 2;
        upperBound = 2;
        Speed = 2;
       
       base.Initialize();
    }

    public override void ApplyEffect()
    {
        Origin.AddStacks(Flow.buffName, Origin.GetBuffStacks(Flow.buffName));
        base.ApplyEffect();
    }

    public override void CardIsUnstaggered()
    {
        base.CardIsUnstaggered();
        Origin.AddStacks(Flow.buffName, 1);
    }

}
