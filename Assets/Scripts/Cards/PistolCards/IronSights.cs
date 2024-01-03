using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronSights : PistolCards
{

    
    public override void ExecuteActionEffect()
    {
        
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        Speed = 2;
        Block = 4;
        Damage = 1;
        lowerBound = 1;
        upperBound = 5;
        accuracy = 1;
        CardType = CardType.RangedAttack;
        myName = "IronSights";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ApplyEffect()
    {
        DupInit();

        Origin.AddStacks(ref duplicateCard, Accuracy.buffName);
        Origin.ApplyBuffsToCard(ref duplicateCard, Accuracy.buffName);
        Origin.UpdateBuffs();
    }

}
