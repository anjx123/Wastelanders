using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFire : PistolCards
{


    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 2;
        upperBound = 3;
        Speed = 2;
        description = "Consume 1 Accuracy, then attack with 'Rapid Fire' again.";
        CardType = CardType.RangedAttack;
        myName = "Rapid Fire";
        Renderer renderer = GetComponent<Renderer>();
        base.Initialize();
    }

    public override void CardIsUnstaggered()
    {
        base.CardIsUnstaggered();
        if (Origin.GetBuffStacks(Accuracy.buffName) > 0)
        {
            Origin.ReduceStacks(Accuracy.buffName, 1);
            BattleQueue.BattleQueueInstance.AddAction(this);
        }
    }

    public override void OnCardStagger()
    {
        if (Origin.GetBuffStacks(Accuracy.buffName) > 0)
        {
            Origin.ReduceStacks(Accuracy.buffName, 1);
            BattleQueue.BattleQueueInstance.AddAction(this);
        }
    }
}