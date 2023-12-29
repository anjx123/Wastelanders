using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolWhip : ActionClass
{

    
    public override void ExecuteActionEffect()
    {
        Debug.Log("Executing Pistol Whip");
    }

    // Start is called before the first frame update
    void Start()
    {
        Speed = 1;
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
