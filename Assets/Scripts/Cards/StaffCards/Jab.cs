using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jab : StaffCards
{
    // Start is called before the first frame update
    public override void Initialize()
    {
        CardType = CardType.MeleeAttack;
        myName = "Jab";
        description = "Attack, then gain 2 Flow.";
        evolutionCriteria = "Win clashes 10 times.";
        evolutionDescription = "Cost 2. Gain 2 flow, then attack.";
        MaxEvolutionProgress = 10;
        lowerBound = 2;
        upperBound = 4;
        Speed = 2;

        base.Initialize();
    }

    public override void OnCardStagger()
    {
        base.OnCardStagger();
        Origin.AddStacks(Flow.buffName, 2);
    }

    public override void CardIsUnstaggered()
    {
        base.CardIsUnstaggered();
        Origin.AddStacks(Flow.buffName, 2);
    }

    public override void ClashWon() {
        base.ClashWon();
        CurrentEvolutionProgress++;
    }
}
