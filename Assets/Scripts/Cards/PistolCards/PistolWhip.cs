using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolWhip : PistolCards
{

    
    public override void OnCardStagger()
    {
        
    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 3;
        upperBound = 3;
        Speed = 1;
        description = "On hit, gain 1 Accuracy stack";
        myName = "Pistol Whip";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
        base.Initialize();
        CardType = CardType.MeleeAttack; //Has to be after otherwise it will get overwritten by superclass
    }

    public override void OnHit()
    {
        base.OnHit();
        Origin.AddStacks(Accuracy.buffName, 1);
    }


}
