using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.UI.Image;

public class Headshot : PistolCards
{
    public override void ExecuteActionEffect()
    {

    }

    // Start is called before the first frame update
    public override void Start()
    {
        lowerBound = 1;
        upperBound = 5;
        base.Start();
        Speed = 5;
        Block = 2;
        Damage = 3;

        myName = "Headshot";
        myDescription = "If This Card Staggers The Opponent, Deal +1 Damage For Each Stack Of Accuracy";
        CardType = CardType.RangedAttack;
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;



    }

    public override void OnHit()
    {
        IncrementRoll(Origin.GetBuffStacks(Accuracy.buffName));
        base.OnHit();
    }
}
