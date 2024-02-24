using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFire : PistolCards
{

    // static int count = 4;
    
    public override void ExecuteActionEffect()
    {
        
    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 4;
        Speed = 2;
        Block = 2;
        Damage = 3;
        description = "Attack, Lose 1 accuracy, if unstaggered, make this attack again.";
        CardType = CardType.MeleeAttack;
        myName = "RapidFire";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
        base.Initialize();
    }

    public override void ApplyEffect()
    {
        base.ApplyEffect();
    }

    public override void CardIsUnstaggered()
    {
        /*        Debug.Log(Origin.GetBuffStacks(Accuracy.buffName));
                Origin.AddStacks(Accuracy.buffName, 10); // for debuggin*/

        if (proto && activeDupCardInstance == null)
        {
            activeDupCardInstance = Instantiate(duplicateCardInstance.GetComponent<RapidFireDuplicate>());
            ((RapidFireDuplicate)activeDupCardInstance).proto = false;
            ((RapidFireDuplicate)activeDupCardInstance).duplicateCardInstance = null;
            activeDupCardInstance.transform.position = new Vector3(-10, 10, 10);
        }

        if (proto && Origin.GetBuffStacks(Accuracy.buffName) > 0) //&& (count > 0)) // doesn't have to be the original card 
        {
            // count--;
            PlayerClass origin = (PlayerClass)Origin;
            activeDupCardInstance.Origin = origin;
            activeDupCardInstance.Target = Target;
            BattleQueue.BattleQueueInstance.InsertDupPlayerAction(activeDupCardInstance);
            Origin.ReduceStacks(Accuracy.buffName, 1); // Reduce Accuracy by 1; only once 

        }
    }

}
