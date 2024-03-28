using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class Pummel : FistCards
{
#nullable enable
    Pummel? activeDuplicateInstance = null;
    int cardVersion = 1;

    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 2;
        Speed = 4;

        myName = "Pummel";
        description = "Make this attack 3 times against the target";
        OriginalPosition = transform.position;
        base.Initialize();
    }

    public override void ApplyEffect()
    {
        if (cardVersion == 1)
        {
            if (activeDuplicateInstance == null)
            {
                activeDuplicateInstance = Instantiate(this.GetComponent<Pummel>());
                activeDuplicateInstance.cardVersion = 2;
                activeDuplicateInstance.transform.position = new Vector3(-10, 10, 10);
                activeDuplicateInstance.Origin = Origin;
                activeDuplicateInstance.Target = Target;
            }
            BattleQueue.BattleQueueInstance.InsertDupPlayerAction(activeDuplicateInstance!);
        }

        if (cardVersion == 2)
        {
            if (activeDuplicateInstance == null)
            {
                activeDuplicateInstance = Instantiate(this.GetComponent<Pummel>());
                activeDuplicateInstance.cardVersion = 3;
                activeDuplicateInstance.transform.position = new Vector3(-10, 10, 10);
                activeDuplicateInstance.Origin = Origin;
                activeDuplicateInstance.Target = Target;
            }
            BattleQueue.BattleQueueInstance.InsertDupPlayerAction(activeDuplicateInstance!);
        }
        base.ApplyEffect();
    }
}
