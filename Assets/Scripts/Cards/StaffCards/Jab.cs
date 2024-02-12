using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jab : StaffCards
{
    public override void ExecuteActionEffect()
    {

    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 2;
        upperBound = 4;

        Speed = 4;
        Block = 2;
        

        myName = "Jab";
        description = "Quick but Lethal";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
        base.Initialize();


    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void ApplyEffect()
    {

        base.ApplyEffect();
    }
}
