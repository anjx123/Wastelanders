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
        upperBound = 5;
        Speed = 1;
        description = "Good Ol' reliable CQC!";
        myName = "Pistol Whip";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
        base.Initialize();
        CardType = CardType.MeleeAttack; //Has to be after otherwise it will get overwritten by superclass
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ApplyEffect()
    {
        base.ApplyEffect();
    }



}
