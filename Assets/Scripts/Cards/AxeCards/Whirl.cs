using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Whirl : AxeCards
{
    public override void Initialize()
    {
        lowerBound = 2;
        upperBound = 4;
        Speed = 3;

        myName = "Whirl";
        description = "Make this attack twice if unstaggered, apply 1 wound if this attack deals damage.";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material;
        OriginalPosition = transform.position;
        base.Initialize();
    }
}
