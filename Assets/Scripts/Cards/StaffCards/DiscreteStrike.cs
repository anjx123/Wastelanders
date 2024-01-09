using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscreteStrike : StaffCards
{
    public override void ExecuteActionEffect()
    {

    }

    // Start is called before the first frame update
    public override void Start()
    {
        lowerBound = 1;
        upperBound = 1;
        base.Start();
        Speed = 1;
        Block = 2;

        myName = "DiscreteStrike";
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

    public override void OnHit()
    {
        base.OnHit();
        Origin.AddStacks(Focus.buffName, 2);
    }
}
