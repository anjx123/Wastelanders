using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFire : PistolCards
{
<<<<<<< HEAD

    private int originalLower;
    private int originalUpper;


=======
    
>>>>>>> parent of f848969 (Merge branch 'main' into md-cards-selection-psound-psettings)
    public override void ExecuteActionEffect()
    {

    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 4;
        originalLower = lowerBound;
        originalUpper = upperBound;
        Speed = 2;
<<<<<<< HEAD
        description = "Attack, if unstaggered make this card again with bounds * min(3, Accuracy), consuming accuracy in process.";
=======
        Block = 2;
        Damage = 3;
        description = "Attack, Lose 1 accuracy, if unstaggered, make this attack again.";
>>>>>>> parent of f848969 (Merge branch 'main' into md-cards-selection-psound-psettings)
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

<<<<<<< HEAD
    public override void OnHit()
    {   
        base.OnHit();
        Debug.Log(Origin.GetBuffStacks(Accuracy.buffName));
        Origin.AddStacks(Accuracy.buffName, 10); // for debuggin
=======
    public override void CardIsUnstaggered()
    {
>>>>>>> parent of f848969 (Merge branch 'main' into md-cards-selection-psound-psettings)

        if (proto && activeDupCardInstance == null)
        {
            activeDupCardInstance = Instantiate(duplicateCardInstance.GetComponent<RapidFireDuplicate>());
            ((RapidFireDuplicate)activeDupCardInstance).proto = false;
            ((RapidFireDuplicate)activeDupCardInstance).duplicateCardInstance = null;
            activeDupCardInstance.transform.position = new Vector3(-10, -10, -10);
<<<<<<< HEAD
        }

        if (proto && Origin.GetBuffStacks(Accuracy.buffName) > 0) // has to be the original card that inserts the duplicate Instance
        {
            ((RapidFireDuplicate)activeDupCardInstance).lowerBound = this.originalLower; // reset it each time
            ((RapidFireDuplicate)activeDupCardInstance).lowerBound = this.originalUpper;
            int incrementValue = 0;
            while (Origin.GetBuffStacks(Accuracy.buffName) > 0 && incrementValue < 3)
            {
                incrementValue++;
                Origin.ReduceStacks(Accuracy.buffName, 1);
            }
            PlayerClass origin = (PlayerClass)Origin;
            activeDupCardInstance.Origin = origin;
            activeDupCardInstance.Target = Target;
            ((RapidFireDuplicate)activeDupCardInstance).lowerBound = incrementValue * lowerBound;
            ((RapidFireDuplicate)activeDupCardInstance).upperBound = incrementValue * upperBound;
            BattleQueue.BattleQueueInstance.InsertDupPlayerAction(activeDupCardInstance);
=======
>>>>>>> parent of f848969 (Merge branch 'main' into md-cards-selection-psound-psettings)
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
<<<<<<< HEAD
=======

>>>>>>> parent of f848969 (Merge branch 'main' into md-cards-selection-psound-psettings)
}
