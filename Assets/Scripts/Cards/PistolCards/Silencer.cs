using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Silencer : PistolCards
{
    public override void OnCardStagger()
    {

    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 4;
        Speed = 3;
        description = "If this Attack hits an opponent, gain 2 accuracy after.";
        myName = "Silencer";
        CardType = CardType.RangedAttack;
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

    public override void OnHit()
    {
        base.OnHit();
        Origin.AddStacks(Accuracy.buffName, 2);

    }
}
