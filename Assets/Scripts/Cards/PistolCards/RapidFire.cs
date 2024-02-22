using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFire : PistolCards
{

    // @Author Muhammad
    protected bool original = true;

    
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

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ApplyEffect()
    {
        base.ApplyEffect();
    }

    public override void CardIsUnstaggered()
    {
/*        Debug.Log(Origin.GetBuffStacks(Accuracy.buffName));
        Origin.AddStacks(Accuracy.buffName, 10); // for debugging*/

        if (Origin.GetBuffStacks(Accuracy.buffName) > 0)
        {
            if (original)
            {
                Origin.ReduceStacks(Accuracy.buffName, 1); // Reduce Accuracy by 1; only once 

                PlayerClass origin = (PlayerClass)Origin;
                List<GameObject> dupActions = origin.GetDuplicates();
                RapidFireDuplicate a = null;
                // a = Instantiate(dupActions[0]).GetComponent<HipFireDuplicate>(); // The object itself has a component of itself
                // a = Instantiate(origin.GetComponent<RapidFireDuplicate>()); // since Origin is a Game Object and it contains it contains this Component somewhere down the line FALSE
                a = Instantiate(origin.GetDuplicates()[1].GetComponent<RapidFireDuplicate>());
                if (a == null)
                {
                    throw new System.Exception("The card Duplicate has either not been defined or is not present inside the duplicates field.");
                }
                a.Origin = origin;
                a.Target = this.Target; // attacking twice against the same target. !!! COULD THROW AN EXCEPTION POTENTIALLY IF THE PLAYER IS DEAD UNLESS PROPER REMOVEINSTANCES ISN'T CALLED
                a.original = false;
                BattleQueue.BattleQueueInstance.InsertDupPlayerAction(a);
            }
        }
    }

}
