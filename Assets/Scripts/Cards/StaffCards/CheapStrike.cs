using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheapStrike : StaffCards
{
    public override void ExecuteActionEffect()
    {

    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 3;
        
        Speed = 1;
        Block = 2;

        myName = "CheapStrike";
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

        DupInit();

        Origin.ApplyAllBuffsToCard(ref duplicateCard);
    }

    public override void OnHit()
    {
        base.OnHit();
        Origin.AddStacks(Focus.buffName, 2);
    }
}
