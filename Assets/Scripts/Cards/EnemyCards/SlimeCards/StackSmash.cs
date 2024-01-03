using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackSmash : SlimeAttacks
{
    public override void ExecuteActionEffect()
    {

    }

    // Start is called before the first frame update
    public override void Start()
    {
        lowerBound = 2;
        upperBound = 4;
        base.Start();
        Speed = 2;
        Block = 2;

        myName = "StackSmash";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
    }

    public override void ApplyEffect()
    {
        DupInit();

        Origin.ApplyBuffsToCard(ref duplicateCard, Accuracy.buffName);
        Origin.ApplyBuffsToCard(ref duplicateCard, Focus.buffName);
    }

    public override void OnHit()
    {
        base.OnHit();
        BattleQueue.BattleQueueInstance.AddPlayerAction(this);
    }
}
