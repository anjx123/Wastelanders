using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HipFire : PistolCards
{

    
    public override void ExecuteActionEffect()
    {
        
    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 5;
        Speed = 1;
        Block = 2;
        Damage = 3;
        description = "Make this attack once, if unstaggered, make it again.";
        CardType = CardType.MeleeAttack;
        myName = "HipFire";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
        base.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ApplyEffect()
    {
        DupInit();
        
        Origin.ApplyAllBuffsToCard(ref duplicateCard);
    }



}
