using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brace : FistCards
{
    public override void ExecuteActionEffect()
    {

    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 2;
        upperBound = 4;
        Speed = 3;

        myName = "Brace";
        description = "Steel Yourself";
        CardType = CardType.Defense;
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
        base.Initialize();
    }

    public override void OnHit()
    {
        base.OnHit();
    }
}
