using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolWhip : PistolCards
{

    
    public override void ExecuteActionEffect()
    {
        
    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 3;
        upperBound = 5;
        Speed = 1;
        Block = 0;
        Damage = 0;
        description = "Good Ol' reliable CQC!";
        CardType = CardType.MeleeAttack;
        myName = "PistolWhip";
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
        base.ApplyEffect();
    }



}
