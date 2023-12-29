using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickDraw : ActionClass
{

    
    public override void ExecuteActionEffect()
    {
        Debug.Log("Executing Quick Draw");
    }

    // Start is called before the first frame update
    void Start()
    {
        Speed = 1;
        Block = 2;
        Damage = 3;
        myName = "QuickDraw";
        CardType = CardType.RangedAttack;
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        CardType = CardType.RangedAttack;
        OriginalPosition = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
