using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PincerPlayable : Pincer, IPlayableBeetleCard
{
#nullable enable
    PincerPlayable? activeDuplicateInstance = null;
    bool originalCopy = true;

    public override void Initialize()
    {
        base.Initialize();
        CostToAddToDeck = 1;
        description = "Pincer, then Pincer again!";
    }

    public override void ApplyEffect()
    {
        if (originalCopy)
        {
            if (activeDuplicateInstance == null)
            {
                activeDuplicateInstance = Instantiate(this.GetComponent<PincerPlayable>());
                activeDuplicateInstance.originalCopy = false;
                activeDuplicateInstance.transform.position = new Vector3(-10, 10, 10);
            }
            activeDuplicateInstance.Origin = Origin;
            activeDuplicateInstance.Target = Target;
            BattleQueue.BattleQueueInstance.AddAction(activeDuplicateInstance!);
        }
        base.ApplyEffect();
    }
}
