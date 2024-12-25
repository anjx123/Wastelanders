using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFire : PistolCards
{
    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 4;
        Speed = 2;
        description = "Consume 1 Accuracy, then attack with 'Rapid Fire' again. On kill, gain 1 accuracy.";
        CardType = CardType.RangedAttack;
        myName = "Rapid Fire";
        Renderer renderer = GetComponent<Renderer>();
        base.Initialize();
    }

    public override void ApplyEffect()
    {
        base.ApplyEffect();
        if (Origin.GetBuffStacks(Accuracy.buffName) > 0)
        {
            Origin.ReduceStacks(Accuracy.buffName, 1);
            BattleQueue.BattleQueueInstance.AddAction(this);
        }
    }

    public override void OnHit()
    {
        base.OnHit();
        if (Target.IsDead)
        {
            Origin.AddStacks(Accuracy.buffName, 1);
        }
    }

    public override void OnCardStagger()
    {
        base.OnCardStagger();
        if (Origin.GetBuffStacks(Accuracy.buffName) > 0)
        {
            Origin.ReduceStacks(Accuracy.buffName, 1);
            BattleQueue.BattleQueueInstance.AddAction(this);
        }
    }
}