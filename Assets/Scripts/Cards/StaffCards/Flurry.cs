using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class Flurry : StaffCards
{
    public override void ExecuteActionEffect()
    {
        Debug.Log("Executing Effect");
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        CardType = CardType.MeleeAttack;
        myName = "Flurry";
        myDescription = "Make This Attack Once, Then Make It Again";
        lowerBound = 2;
        upperBound = 4;
        Speed = 3;

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
