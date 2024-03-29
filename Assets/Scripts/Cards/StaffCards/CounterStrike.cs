using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.UI.Image;

public class CounterStrike : StaffCards
{
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
    }

    public override void ApplyEffect()
    {
        if (proto && activeDupCardInstance == null)
        {
            activeDupCardInstance = Instantiate(this.GetComponent<CounterStrike>());
            ((CounterStrike)activeDupCardInstance).proto = false;
            ((CounterStrike)activeDupCardInstance).duplicateCardInstance = null;
            ((CounterStrike)activeDupCardInstance).CardType = CardType.MeleeAttack;
            activeDupCardInstance.transform.position = new Vector3(-10, -10, -10);
        }

        if (proto)
        {
            CardType = CardType.Defense;
            PlayerClass origin = (PlayerClass)Origin;
            activeDupCardInstance.Origin = origin;
            activeDupCardInstance.Target = Target;
            BattleQueue.BattleQueueInstance.InsertDupPlayerAction(activeDupCardInstance);
        }
        base.ApplyEffect();
    }

}
