using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Whirl : AxeCards
{
#nullable enable
    Whirl? activeDuplicateInstance = null;
    bool originalCopy = true;

    public override void Initialize()
    {
        lowerBound = 2;
        upperBound = 4;
        Speed = 3;

        myName = "Whirl";
        description = "Make this attack, if unstaggered, apply one wound then make it again.";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material;
        OriginalPosition = transform.position;
        base.Initialize();
    }

    // for the "deals damage" portion how will you know? there is no reference to the enemy nor the outcome of a clash. 
    public override void CardIsUnstaggered()
    {
        if (originalCopy)
        {
            if (activeDuplicateInstance == null)
            {
                activeDuplicateInstance = Instantiate(this.GetComponent<Whirl>());
                activeDuplicateInstance.originalCopy = false;
                activeDuplicateInstance.transform.position = new Vector3(-10, 10, 10);
            }
            activeDuplicateInstance.Origin = Origin;
            activeDuplicateInstance.Target = Target;
            BattleQueue.BattleQueueInstance.InsertDupPlayerAction(activeDuplicateInstance!);
        }

        Target.AddStacks(Wound.buffName, 1);
        base.CardIsUnstaggered();
    }

}
