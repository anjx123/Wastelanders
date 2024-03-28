using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

// REQUIRES TESTING TODO (Haven't done this yet owing to the dearth of staff user presumably Ives.
public class Flurry : StaffCards
{

#nullable enable
    Flurry? activeDuplicateInstance = null;
    bool originalCopy = true;

    // Start is called before the first frame update
    public override void Initialize()
    {
        
        CardType = CardType.MeleeAttack;
        myName = "Flurry";
        description = "Make This Attack Once, Then Make It Again";
        lowerBound = 2;
        upperBound = 4;
        Speed = 3;

        base.Initialize();
    }

    public override void ApplyEffect()
    {
        if (originalCopy)
        {
            if (activeDuplicateInstance == null)
            {
                activeDuplicateInstance = Instantiate(this.GetComponent<Flurry>());
                activeDuplicateInstance.originalCopy = false;
                activeDuplicateInstance.transform.position = new Vector3(-10, 10, 10);
                activeDuplicateInstance.Origin = Origin;
                activeDuplicateInstance.Target = Target;
            }
            BattleQueue.BattleQueueInstance.InsertDupPlayerAction(activeDuplicateInstance!);
        }
        base.ApplyEffect();
    }
}
