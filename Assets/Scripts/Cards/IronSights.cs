using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronSights : PistolCards
{

    
    public override void ExecuteActionEffect()
    {
        
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        Speed = 2;
        Block = 4;
        Damage = 1;
        CardType = CardType.RangedAttack;
        myName = "IronSights";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
