using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusedStrike : StaffCards
{
    // Start is called before the first frame update
    public override void Initialize()
    {
        CardType = CardType.MeleeAttack;
        myName = "Focused Strike";
        description = "Double your Flow then make this attack. On hit, gain 1 Flow.";
        lowerBound = 2;
        upperBound = 2;
        Speed = 2;
        CostToAddToDeck = IsEvolved ? 4 : 2;
        evolutionCriteria = "Strike for a power of 8+ with this card three times.";
        evolutionDescription = "Double your flow then make this attack, this card does not consume flow";
        MaxEvolutionProgress = 3;
       
       base.Initialize();
    }

    public override void ApplyEffect()
    {
        Origin.AddStacks(Flow.buffName, Origin.GetBuffStacks(Flow.buffName));
        base.ApplyEffect();
    }

    public override void OnHit()
    {
        base.OnHit();
        Origin.AddStacks(Flow.buffName, 1);
        if (getRolledDamage() >= 8) {
            CurrentEvolutionProgress++;
        }
    }

}
