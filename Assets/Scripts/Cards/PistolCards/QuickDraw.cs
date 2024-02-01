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
        lowerBound = 1;
        upperBound = 5;
        base.Start();
        Speed = 5;
        Block = 2;
        Damage = 3;

        myName = "QuickDraw";
        myDescription = "They’ll Never See It Coming!";
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

        base.ApplyEffect();
        
    }

}
