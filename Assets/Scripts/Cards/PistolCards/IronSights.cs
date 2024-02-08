using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronSights : PistolCards
{

    
    public override void ExecuteActionEffect()
    {
        
    }


    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 3;

        Speed = 2;
        Block = 4;
        Damage = 1;


        CardType = CardType.RangedAttack;
        myName = "IronSights";
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

        Origin.AddStacks(Accuracy.buffName, 1);
        Origin.ApplyAllBuffsToCard(ref duplicateCard);
    }

}
