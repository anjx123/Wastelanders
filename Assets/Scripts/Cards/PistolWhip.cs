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
        speed = 1;
        block = 2;
        damage = 99;
        myName = "PistolWhip";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
