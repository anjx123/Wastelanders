using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PincerPlayable : Pincer, IPlayableBeetleCard
{
#nullable enable
    [SerializeField] private AnimationClip? pincerClip;
    PincerPlayable? activeDuplicateInstance = null;
    bool originalCopy = true;

    public override void Initialize()
    {
        base.Initialize();
        CostToAddToDeck = 1;
        description = "Pincer, then Pincer again!";
    }

    public override void CardIsUnstaggered()
    {
        base.CardIsUnstaggered();
        IPlayableEnemyCard.ApplyForeignAttackAnimation(Origin, pincerClip, PINCER_ANIMATION_NAME);
        Origin.AttackAnimation(PINCER_ANIMATION_NAME);
        InsertDuplicate();
    }

    public override void OnCardStagger()
    {
        base.OnCardStagger();
        InsertDuplicate();
    }

    private void InsertDuplicate()
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
    }
}
