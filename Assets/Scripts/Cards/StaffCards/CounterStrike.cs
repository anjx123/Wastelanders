using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.UI.Image;

public class CounterStrike : StaffCards
{
#nullable enable
    CounterStrike? activeDuplicateInstance = null;
    private bool originalCopy = true;
    public override void Initialize()
    {
        lowerBound = 2;
        upperBound = 4;
        Speed = 4;

        myName = "Counter Strike";
        description = "Block, then make an attack with this card. Each unstaggered defense/attack grants 1 Flow.";
        base.Initialize();
    }

    public override void ApplyEffect()
    {
        if (originalCopy)
        {
            CardType = CardType.Defense;
            if (activeDuplicateInstance == null)
            {
                activeDuplicateInstance = Instantiate(this.GetComponent<CounterStrike>());
                activeDuplicateInstance.originalCopy = false;
                activeDuplicateInstance.transform.position = new Vector3(-10, 10, 10);
                activeDuplicateInstance.CardType = CardType.MeleeAttack;
            }
            activeDuplicateInstance.Origin = Origin;
            activeDuplicateInstance.Target = Target;
            BattleQueue.BattleQueueInstance.AddAction(activeDuplicateInstance!);
        }
        base.ApplyEffect();
    }

    public override void CardIsUnstaggered()
    {
        Origin.AddStacks(Flow.buffName, 1);
        base.CardIsUnstaggered();

    }

}
