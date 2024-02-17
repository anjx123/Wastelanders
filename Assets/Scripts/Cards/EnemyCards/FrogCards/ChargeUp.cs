using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.UI.Image;

public class ChargeUp : FrogAttacks
{
    public override void ExecuteActionEffect()
    {

    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        base.Initialize();
        lowerBound = 1;
        upperBound = 3;
        
        Speed = 1;
        Block = 2;

        description = "If this ability is unstaggered, use 'Hurl' next turn";

        myName = "ChargeUp";
        CardType = CardType.Defense;
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
    }

    public override void ApplyEffect()
    {
        DupInit();

        Origin.ApplyAllBuffsToCard(ref duplicateCard);
    }
    
    // @Author Muhammad 
    // Inserts Hurl Next Turn.
    public override void OnHit()
    {
        WasteFrog frog = (WasteFrog )this.Origin;
        frog.UseHurl = true;
    }
}
