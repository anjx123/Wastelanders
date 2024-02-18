using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class Flurry : StaffCards
{
    public override void ExecuteActionEffect()
    {
        Debug.Log("Executing Effect");
    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        
        CardType = CardType.MeleeAttack;
        myName = "Flurry";
        description = "Make This Attack Once, Then Make It Again";
        lowerBound = 2;
        upperBound = 4;
        Speed = 3;

        base.Initialize();
    }



    public override void CardIsUnstaggered()
    {
        //TODO: Reinsert this card into the battlequeue
    }
}
