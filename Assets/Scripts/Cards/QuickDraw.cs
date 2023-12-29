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
        base.Start();
        Speed = 1;
        Block = 2;
        Damage = 3;
        myName = "QuickDraw";
        CardType = CardType.RangedAttack;
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
