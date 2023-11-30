using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusedStrike : ActionClass
{
    public override void ExecuteActionEffect()
    {
        Debug.Log("Executing Effect");
    }

    // Start is called before the first frame update
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card, maybe refac into ActionClass?  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
