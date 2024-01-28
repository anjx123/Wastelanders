using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class Spit : FrogAttacks
{
    public override void ExecuteActionEffect()
    {

    }

    public override void Initialize()
    {
        base.Initialize();
        lowerBound = 1;
        upperBound = 4;
        Speed = 4;
        Block = 2;

        myName = "Spit";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
    }

    public override void ApplyEffect()
    {
        DupInit();

        Origin.ApplyAllBuffsToCard(ref duplicateCard);
    }
}
