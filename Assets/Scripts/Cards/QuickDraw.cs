using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickDraw : PistolCards
{

    
    public override void ExecuteActionEffect()
    {
        
    }

    // Start is called before the first frame update
    public override void Start()
    {
        lowerBound = 1;
        upperBound = 4;
        cardType = true;
        focus = 0;
        accuracy = 2;
        base.Start();
        Speed = 1;
        Block = 2;
        Damage = 3;

        myName = "QuickDraw";
        CardType = CardType.RangedAttack;
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
        Debug.Log(Accuracy.buffName + Origin.getName() + duplicateCard.rollCeiling);

        if (Origin == null)
        {
            Debug.Log("failure");
        }

        Origin.AddStacks(ref duplicateCard, Accuracy.buffName);
        Origin.ApplyBuffsToCard(ref duplicateCard, Accuracy.buffName);
        RollDice();
    }

}
