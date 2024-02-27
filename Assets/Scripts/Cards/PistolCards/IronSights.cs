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
        upperBound = 4;
	Speed = 2;

        CardType = CardType.RangedAttack;
        myName = "IronSights";
        description = "Gain One Accuracy";
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
        

        Origin.AddStacks(Accuracy.buffName, 1);
        base.ApplyEffect();
    }

}
