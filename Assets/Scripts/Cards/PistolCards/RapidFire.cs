using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFire : PistolCards
{

    
    public override void ExecuteActionEffect()
    {
        
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        Speed = 1;
        Block = 0;
        Damage = 5;
        CardType = CardType.MeleeAttack;
        myName = "RapidFire";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
