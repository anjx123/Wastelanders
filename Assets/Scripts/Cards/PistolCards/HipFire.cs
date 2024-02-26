
using System.Collections;

using System.Collections.Generic;
using UnityEngine;

public class HipFire : PistolCards
{
    public override void ExecuteActionEffect()
    {
        Debug.Log("Executing Effect");
    }

    public override void OnHit()
    {
        RollDice();
        base.OnHit();
    }


    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 5;
        Speed = 1;
        description = "Make This Attack Once, If Unstaggered, Make It Again";
        CardType = CardType.RangedAttack;
        myName = "HipFire";

        base.Initialize();
    }


    public override void ApplyEffect()
    {
        DupInit();
        
        Origin.ApplyAllBuffsToCard(ref duplicateCard);
    }

}
