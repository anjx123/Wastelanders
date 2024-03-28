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
        description = "Gain 1 Focus, Then Attack";
        lowerBound = 2;
        upperBound = 2;
        Speed = 2;
       
       base.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ApplyEffect()
    {     


        Origin.AddStacks(Focus.buffName, 1);
        base.ApplyEffect();
    }

}
