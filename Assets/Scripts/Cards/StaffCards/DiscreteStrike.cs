using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscreteStrike : StaffCards
{
    public override void OnCardStagger()
    {

    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 1;
        Speed = 1;

        myName = "Discrete Strike";
        description = "Gain 2 focus, then strike";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
        CardType = CardType.MeleeAttack;
        base.Initialize();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void ApplyEffect()
    {
        Origin.AddStacks(Focus.buffName, 2);
        base.ApplyEffect();
    }

    public override void OnHit()
    {
        base.OnHit();
    }
}
