using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class LeftHook : FistCards
{
    public override void ExecuteActionEffect()
    {

    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 2;
        upperBound = 3;
        Speed = 2;

        myName = "Left Hook";
        description = "Hook to the Left";
        
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
        base.Initialize();
    }

    public override void OnHit()
    {
        base.OnHit();
    }
}
