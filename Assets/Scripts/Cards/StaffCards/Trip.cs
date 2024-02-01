using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class Trip : StaffCards
{
    public override void ExecuteActionEffect()
    {
        Debug.Log("Executing Effect");
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        CardType = CardType.MeleeAttack;
        myName = "Trip";
        myDescription = "If This Attack Staggers The Opponent, Gain 1 Focus";
        lowerBound = 2;
        upperBound = 4;
        Speed = 4;

        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card, maybe refac into ActionClass?
        OriginalPosition = transform.position;
    }

    public override void OnHit()
    {
        base.OnHit();
        Origin.AddStacks(Focus.buffName, 1);
    }
}
