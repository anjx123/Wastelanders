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
        CardType = CardType.MeleeAttack;
        myName = "FocusedStrike";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card, maybe refac into ActionClass?
        OriginalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}