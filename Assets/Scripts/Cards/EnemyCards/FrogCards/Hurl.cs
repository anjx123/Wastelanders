using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.UI.Image;

public class Hurl : FrogAttacks, IPlayableFrogCard
{

    public static string HURL_NAME = "Hurl";

    // Start is called before the first frame update
    public override void Initialize()
    {
        base.Initialize();
        lowerBound = 3;
        upperBound = 7;
        
        Speed = 5;

        CostToAddToDeck = 2;

        myName = "Hurl";
        description = "Watch out!";
        Renderer renderer = GetComponent<Renderer>();
    }


}
