using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Silencer : PistolCards
{
    public override void ExecuteActionEffect()
    {

    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 4;
        Speed = 3;
        Block = 2;
        description = "If this Attack Staggers the opponent, gain 2 accuracy.";
        myName = "Silencer";
        myDescription = "If this Attack Staggers the Opponent, Gain 2 Accuracy";
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
