using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronSights : ActionClass
{

    
    public override void ExecuteActionEffect()
    {
        Debug.Log("Executing Iron Sights");
    }

    // Start is called before the first frame update
    void Start()
    {
        speed = 2;
        block = 4;
        damage = 1;
        myName = "IronSights";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}