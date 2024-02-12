using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickDraw : PistolCards
{

    
    public override void ExecuteActionEffect()
    {
        
    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1; // MAKE SURE BOUNDS ARE SET BEFORE CALLING SUPERCLASS START
        upperBound = 5;
        Speed = 5;
        Block = 2;
        Damage = 3;
        description = "Theyâ€™ll never see it coming!";
        myName = "QuickDraw";
        myDescription = "They’ll Never See It Coming!";
        CardType = CardType.RangedAttack;
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
