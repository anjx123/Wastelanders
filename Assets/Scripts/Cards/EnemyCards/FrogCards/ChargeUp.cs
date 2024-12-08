using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.UI.Image;

public class ChargeUp : FrogAttacks
{
    // Start is called before the first frame update
    public override void Initialize()
    {
        base.Initialize();
        lowerBound = 1;
        upperBound = 3;
        
        Speed = 1;

        description = "Block, if unstaggered, use 'Hurl' next turn";

        myName = "Charge Up";
        CardType = CardType.Defense;
        Renderer renderer = GetComponent<Renderer>();
    }



    public override void CardIsUnstaggered()
    {
        WasteFrog frog = (WasteFrog)this.Origin;
        frog.UseHurl = true;
        Origin.AttackAnimation("IsBlocking"); // :3
        
    }

}
