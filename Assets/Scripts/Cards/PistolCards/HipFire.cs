
using System.Collections;

using System.Collections.Generic;
using UnityEngine;

// Minor note: there is inconsisteny in the script naming: Hip Fire != HipFire; I added the space for the duplicates on purpose 
public class HipFire : PistolCards
{
#nullable enable
    HipFire? activeDuplicateInstance = null;
    bool originalCopy = true;

    public override void CardIsUnstaggered()
    {
        base.CardIsUnstaggered();
        InsertDuplicate();
    }

    public override void OnCardStagger()
    {
        base.OnCardStagger();
        InsertDuplicate();
    }

    private void InsertDuplicate()
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
    }


    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 4;
        Speed = 3;
        description = "Make this attack once, then make it again.";
        CardType = CardType.RangedAttack;
        myName = "Hip Fire";

        base.Initialize();
    }
}
