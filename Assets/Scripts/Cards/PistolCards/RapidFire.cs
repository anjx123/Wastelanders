using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFire : PistolCards
{
    
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

        if (proto && activeDupCardInstance == null)
        {
            activeDupCardInstance = Instantiate(duplicateCardInstance.GetComponent<RapidFireDuplicate>());
            ((RapidFireDuplicate)activeDupCardInstance).proto = false;
            ((RapidFireDuplicate)activeDupCardInstance).duplicateCardInstance = null;
            activeDupCardInstance.transform.position = new Vector3(-10, -10, -10);
        }

/*        Origin.AddStacks(Accuracy.buffName, 10); // for debuggin
        Debug.Log(Origin.GetBuffStacks(Accuracy.buffName));*/

        if (proto && Origin.GetBuffStacks(Accuracy.buffName) > 0) //&& (count > 0)) // doesn't have to be the original card 
        {
            ((RapidFireDuplicate)activeDupCardInstance).Damage = 0; // reset it each time
            int damageValue = 0;
            while (Origin.GetBuffStacks(Accuracy.buffName) > 0 && damageValue < 3)
            {
                damageValue++;
                Origin.ReduceStacks(Accuracy.buffName, 1);
            }
            PlayerClass origin = (PlayerClass)Origin;
            activeDupCardInstance.Origin = origin;
            activeDupCardInstance.Target = Target;
            ((RapidFireDuplicate)activeDupCardInstance).Damage = damageValue * Damage;
            // TODO should I update the upper and lower bounds? 
            BattleQueue.BattleQueueInstance.InsertDupPlayerAction(activeDupCardInstance);
        }

    }

}
