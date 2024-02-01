using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HipFire : PistolCards
{
    public override void ExecuteActionEffect()
    {
        Debug.Log("Executing Effect");
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        CardType = CardType.RangedAttack;
        myName = "HipFire";
        myDescription = "Make This Attack Once, If Unstaggered, Make It Again";
        lowerBound = 1;
        upperBound = 5;
        Speed = 1;

        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card, maybe refac into ActionClass?
        OriginalPosition = transform.position;
    }

    public override void OnHit()
    {
        base.OnHit();
        RollDice();
        base.OnHit();
    }
}
