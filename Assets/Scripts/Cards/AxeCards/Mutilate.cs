using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mutilate : AxeCards
{
    public override void Initialize()
    {
        lowerBound = 2;
        upperBound = 5;
        Speed = 1;

        myName = "Mutilate";
        description = "If this attack deals damage, for the rest of this turn, inflict 1 wound every time the target takes damage.";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material;
        OriginalPosition = transform.position;
        base.Initialize();
    }
}
