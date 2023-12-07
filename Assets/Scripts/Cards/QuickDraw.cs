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
        lowerBound = 1;
        upperBound = 4;
        cardType = true;
        speed = 1;
        // block = 2;
        damage = 3;
        focus = 0;
        accuracy = 2;
        myName = "QuickDraw";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ApplyEffect()
    {

        DupInit();
        Debug.Log(AccuracyScriptableObject.buffName + Origin.getName() + duplicateCard.rollCeiling);

        if (Origin == null)
        {
            Debug.Log("failure");
        }

        Origin.AddBuffs(ref duplicateCard, AccuracyScriptableObject.buffName);
        RollDice();
    }

}
