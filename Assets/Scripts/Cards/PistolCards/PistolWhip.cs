using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolWhip : PistolCards
{

    
    public override void ExecuteActionEffect()
    {
        
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        Speed = 3;
        Block = 2;
        Damage = 99;
        CardType = CardType.MeleeAttack;
        myName = "PistolWhip";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
