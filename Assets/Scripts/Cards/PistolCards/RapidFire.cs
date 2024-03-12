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
        upperBound = 3;
        Speed = 2;
        description = "If unstaggered, consume 1 accuracy, then make another rapid fire attack"; // () => {accuracy > 3 ? 3 : accuracy} // assume accuracy is deducted
        CardType = CardType.RangedAttack;
        myName = "RapidFire";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
        base.Initialize();
    }

    public override void CardIsUnstaggered()
    {
        if (Origin.GetBuffStacks(Accuracy.buffName) > 0)
        {
            Origin.ReduceStacks(Accuracy.buffName, 1);
            BattleQueue.BattleQueueInstance.AddPlayerAction(this);
        }
    }
}