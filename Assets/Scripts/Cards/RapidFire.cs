using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFire : ActionClass
{

    
    public override void ExecuteActionEffect()
    {
        Debug.Log("Executing Rapid Fire");
    }

    // Start is called before the first frame update
    void Start()
    {
        speed = 1;
        block = 2;
        damage = 3;
        myName = "RapidFire";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
