
using System.Collections;

using System.Collections.Generic;
using UnityEngine;

// Minor note: there is inconsisteny in the script naming: Hip Fire != HipFire; I added the space for the duplicates on purpose 
public class HipFire : PistolCards
{

    public override void ExecuteActionEffect()
    {
        Debug.Log("Executing Effect");
    }


    // @Author Muhammad
    public override void CardIsUnstaggered()
    {
        if (proto && activeDupCardInstance == null)
        {
            activeDupCardInstance = Instantiate(duplicateCardInstance.GetComponent<HipFireDuplicate>());
            ((HipFireDuplicate)activeDupCardInstance).proto = false;
            ((HipFireDuplicate)activeDupCardInstance).duplicateCardInstance = null;
            activeDupCardInstance.transform.position = new Vector3(-10, -10, -10);
        }

        if (proto)
        {
            PlayerClass origin = (PlayerClass)Origin;
            activeDupCardInstance.Origin = origin;
            activeDupCardInstance.Target = Target;
            BattleQueue.BattleQueueInstance.InsertDupPlayerAction(activeDupCardInstance);
        }
    }


    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 5;
        Speed = 1;
        Damage = 3;
        description = "Make This Attack Once, If Unstaggered, Make It Again";
        CardType = CardType.RangedAttack;
        myName = "HipFire";

        base.Initialize();
    }


    public override void ApplyEffect()
    {
        DupInit();
        
        Origin.ApplyAllBuffsToCard(ref duplicateCard);
    }

}
