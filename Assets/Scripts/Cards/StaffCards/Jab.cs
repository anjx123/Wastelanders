using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jab : StaffCards
{
    public override void OnCardStagger()
    {

    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 2;
        upperBound = 4;

        Speed = 4;
        

        myName = "Jab";
        description = "Quick but Lethal";
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

        base.ApplyEffect();
    }
}
