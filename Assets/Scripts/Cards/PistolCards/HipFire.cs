
using System.Collections;

using System.Collections.Generic;
using UnityEngine;

// Minor note: there is inconsisteny in the script naming: Hip Fire != HipFire; I added the space for the duplicates on purpose 
public class HipFire : PistolCards
{
#nullable enable
    HipFire? activeDuplicateInstance = null;
    bool originalCopy = true;

    // @Author Muhammad
    public override void CardIsUnstaggered()
    {
        if (originalCopy)
        {
            if (activeDuplicateInstance == null)
            {
                activeDuplicateInstance = Instantiate(this.GetComponent<HipFire>());
                activeDuplicateInstance.originalCopy = false;
                activeDuplicateInstance.transform.position = new Vector3(-10, 10, 10);
            }
            activeDuplicateInstance.Origin = Origin;
            activeDuplicateInstance.Target = Target;
            BattleQueue.BattleQueueInstance.AddAction(activeDuplicateInstance!);
        }
        base.CardIsUnstaggered();
    }


    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 4;
        Speed = 1;
        description = "Make this attack once, if unstaggered, make it once again.";
        CardType = CardType.RangedAttack;
        myName = "Hip Fire";

        base.Initialize();
    }
}
