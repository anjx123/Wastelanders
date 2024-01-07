using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.UI.Image;

public class Hurl : FrogAttacks
{
    public override void ExecuteActionEffect()
    {

    }

    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        lowerBound = 3;
        upperBound = 7;
        
        Speed = 5;
        Block = 2;

        myName = "Hurl";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
    }

    public override void ApplyEffect()
    {
        DupInit();

        Origin.ApplyBuffsToCard(ref duplicateCard, Accuracy.buffName);
        Origin.ApplyBuffsToCard(ref duplicateCard, Focus.buffName);
    }
}
