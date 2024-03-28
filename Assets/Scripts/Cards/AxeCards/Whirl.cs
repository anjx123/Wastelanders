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

    // for the "deals damage" portion how will you know? there is no reference to the enemy nor the outcome of a clash. 
    public override void CardIsUnstaggered()

    {
        if (proto && activeDupCardInstance == null)
        {
            activeDupCardInstance = Instantiate(duplicateCardInstance.GetComponent<WhirlDuplicate>());
            ((WhirlDuplicate)activeDupCardInstance).proto = false;
            ((WhirlDuplicate)activeDupCardInstance).duplicateCardInstance = null;
            activeDupCardInstance.transform.position = new Vector3(-10, 10, 10);
        }

        if (proto)
        {
            PlayerClass origin = (PlayerClass)Origin;
            activeDupCardInstance.Origin = origin;
            activeDupCardInstance.Target = Target;
            BattleQueue.BattleQueueInstance.InsertDupPlayerAction(activeDupCardInstance);
        } 

        base.CardIsUnstaggered();
    }

}
