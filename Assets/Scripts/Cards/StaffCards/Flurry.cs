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
        description = "Make this attack once, then make it attack again. Each attack on hit grants 1 Flow.";
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
            }
            activeDuplicateInstance.Origin = Origin;
            activeDuplicateInstance.Target = Target;
            BattleQueue.BattleQueueInstance.InsertDupPlayerAction(activeDuplicateInstance!);
        }
        base.ApplyEffect();
    }

    public override void OnHit()
    {
        base.OnHit();
        Origin.AddStacks(Flow.buffName, 1);
    }
}


/*
 * 
 *"Consume all Flow stacks, make an extra attack for each Flow consumed code
 *
 * public override void ApplyEffect()
    {
        flowConsumed = Origin.GetBuffStacks(Flow.buffName);
        Origin.ReduceStacks(Flow.buffName, flowConsumed);

        if (originalCopy)
        {
            if (activeDuplicateInstance == null)
            {
                activeDuplicateInstance = Instantiate(this.GetComponent<Flurry>());
                activeDuplicateInstance.originalCopy = false;
                activeDuplicateInstance.transform.position = new Vector3(-10, 10, 10);
            }
            activeDuplicateInstance.flowConsumed = this.flowConsumed;
            activeDuplicateInstance.Origin = Origin;
            activeDuplicateInstance.Target = Target;
            if (activeDuplicateInstance.flowConsumed > 0)
            {
                BattleQueue.BattleQueueInstance.InsertDupPlayerAction(activeDuplicateInstance!);
                activeDuplicateInstance.flowConsumed -= 1;
            }
        } else
        {
            if (flowConsumed > 0)
            {
                BattleQueue.BattleQueueInstance.InsertDupPlayerAction(this!);
                flowConsumed -= 1;
            }
        }
        base.ApplyEffect();
    }
 */
