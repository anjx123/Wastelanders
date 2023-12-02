using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkingStrike : ActionClass
{

    
    public override void ExecuteActionEffect()
    {
        Debug.Log("Executing Quick Draw");
    }

    // Start is called before the first frame update
    void Start()
    {
        speed = 3;
        blockActual = 2;
        damageActual = 5;
        myName = "QuickDraw";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
