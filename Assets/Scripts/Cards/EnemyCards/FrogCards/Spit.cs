using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class Spit : FrogAttacks
{
    public override void OnCardStagger()
    {

    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        base.Initialize();
        lowerBound = 1;
        upperBound = 4;
        Speed = 4;
        CostToAddToDeck = 1;

        myName = "Spit";
        description = "Gross!";
        Renderer renderer = GetComponent<Renderer>();
    }


}
