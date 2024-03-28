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
    public override void OnCardStagger()
    {

    }
    
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 3;
        Speed = 5;

        myName = "Counter Strike";
        description = "Block with this card, then make an attack with this card";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
        base.Initialize();
        CardType = CardType.MeleeAttack;
    }

    public override void ApplyEffect()
    {
        if (originalCopy)
        {
            if (activeDuplicateInstance == null)
            {
                activeDuplicateInstance = Instantiate(this.GetComponent<CounterStrike>());
                activeDuplicateInstance.originalCopy = false;
                activeDuplicateInstance.transform.position = new Vector3(-10, 10, 10);
                activeDuplicateInstance.Origin = Origin;
                activeDuplicateInstance.Target = Target;
                activeDuplicateInstance.CardType = CardType.Defense;
            }
            BattleQueue.BattleQueueInstance.InsertDupPlayerAction(activeDuplicateInstance!);
        }
        base.ApplyEffect();
    }

}
