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
        description = "On hit, gain 2 Flow.";
        evolutionCriteria = "Win clashes 10 times.";
        evolutionDescription = "Cost 2. Gain 2 flow, then attack.";
        MaxEvolutionProgress = 10;
        lowerBound = 2;
        upperBound = 4;
        Speed = 4;

        base.Initialize();
    }

    public override void OnHit()
    {
        if (IsEvolved) {
            Origin.AddStacks(Flow.buffName, 2);
        }
        base.OnHit();
        if (!IsEvolved) {
            Origin.AddStacks(Flow.buffName, 2);
        }
    }

    public override void ClashWon() {
        base.ClashWon();
        CurrentEvolutionProgress++;
    }
}
