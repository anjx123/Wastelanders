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
    public override void Start()
    {
        CardType = CardType.MeleeAttack;
        myName = "FocusedStrike";
        lowerBound = 2;
        upperBound = 3;
       
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card, maybe refac into ActionClass?
        OriginalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ApplyEffect()
    {
        DupInit();

        Origin.AddStacks(Focus.buffName, 1);
        Origin.ApplyAllBuffsToCard(ref duplicateCard);
    }

}
