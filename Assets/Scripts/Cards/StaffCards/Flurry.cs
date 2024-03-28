using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

// REQUIRES TESTING TODO (Haven't done this yet owing to the dearth of staff user presumably Ives.
public class Flurry : StaffCards
{

    public override void OnCardStagger()
    {
        Debug.Log("Executing Effect");
    }

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
        if (proto && activeDupCardInstance == null)
        {
            activeDupCardInstance = Instantiate(duplicateCardInstance.GetComponent<FlurryDuplicate>());
            ((FlurryDuplicate)activeDupCardInstance).proto = false;
            ((FlurryDuplicate)activeDupCardInstance).duplicateCardInstance = null;
            activeDupCardInstance.transform.position = new Vector3(-10, 10, 10);
        }

        if (proto)
        {
            PlayerClass origin = (PlayerClass)Origin;
            activeDupCardInstance.Origin = origin;
            activeDupCardInstance.Target = Target;
            BattleQueue.BattleQueueInstance.InsertDupPlayerAction(activeDupCardInstance);
        }
        base.ApplyEffect();
    }
}
