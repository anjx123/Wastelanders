using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.UI.Image;

public class Headshot : PistolCards
{

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 5;
        Speed = 2;

        myName = "Headshot";
        description = "On hit, deal +1 damage for each stack of Accuracy and gain one accuracy.";
        CardType = CardType.RangedAttack;
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
        base.Initialize();
    }

    public override void OnHit()
    {
        IncrementRoll(Origin.GetBuffStacks(Accuracy.buffName));
        base.OnHit();
        Origin.AddStacks(Accuracy.buffName, 1);
    }
}
