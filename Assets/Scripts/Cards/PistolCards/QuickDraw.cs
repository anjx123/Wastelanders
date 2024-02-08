using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickDraw : PistolCards
{

    
    public override void ExecuteActionEffect()
    {
        
    }

    // Start is called before the first frame update
    public override void Start()
    {
        lowerBound = 1; // MAKE SURE BOUNDS ARE SET BEFORE CALLING SUPERCLASS START
        upperBound = 4;
        base.Start();
        Speed = 5;
        Block = 2;
        Damage = 3;

        myName = "QuickDraw";
        CardType = CardType.RangedAttack;
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;

        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ApplyEffect()
    {

        DupInit();
        

        if (Origin == null)
        {
            Debug.Log("failure");
        }
        
        Origin.ApplyAllBuffsToCard(ref duplicateCard);
        
    }

}
