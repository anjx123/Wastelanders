using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.UI.Image;

public class Pincer : BeetleAttacks
{
    public override void ExecuteActionEffect()
    {

    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        base.Initialize();
        lowerBound = 1;
        upperBound = 4;
        
        Speed = 3;

        description = "Ouch!";

        myName = "Pincer";
        CardType = CardType.MeleeAttack;
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
    }

}
