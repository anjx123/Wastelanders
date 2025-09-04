using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.UI.Image;

public class Headshot : PistolCards
{

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 4;
        Speed = 2;

        myName = "Headshot";
        description = "On hit, deal +1 damage for each stack of Accuracy and gain 1 accuracy on kill.";
        CardType = CardType.RangedAttack;
        base.Initialize();
    }

    public override void OnHit()
    {
        int accuracyStacks = Origin.GetBuffStacks(Accuracy.buffName);
        IncrementRoll(accuracyStacks);
        AudioManager.Instance?.PlaySFX(PISTOL_SOUND_FX_NAME);
        Vector3 diffInLocation = Target.myTransform.position - Origin.myTransform.position;
        Origin.UpdateFacing(diffInLocation, null);
        CardIsUnstaggered();
        Debug.Log("Damage is: " + rolledCardStats.ActualRoll);
        if (rolledCardStats.ActualRoll == accuracyStacks)
        {
            this.Target.TakeDamageNoStagger(Origin, rolledCardStats.ActualRoll);
        } else
        {
            this.Target.TakeDamage(Origin, rolledCardStats.ActualRoll);
        }

        if (Target.IsDead)
        {
            Origin.AddStacks(Accuracy.buffName, 1);
        }
    }
}
