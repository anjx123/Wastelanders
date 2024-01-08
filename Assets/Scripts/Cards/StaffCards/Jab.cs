using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jab : StaffCards
{
    public override void ExecuteActionEffect()
    {

    }

    // Start is called before the first frame update
    public override void Start()
    {
        lowerBound = 2;
        upperBound = 3;
        base.Start();
        Speed = 4;
        Block = 2;
        

        myName = "Jab";
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

       
        Origin.ApplyAllBuffsToCard(ref duplicateCard);
    }
}
