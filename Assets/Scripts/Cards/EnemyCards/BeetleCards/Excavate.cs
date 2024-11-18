using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.UI.Image;

public class Excavate : BeetleAttacks
{
    public const string EXCAVATE_SOUND_EFFECT_NAME = "Excavate Cut";


    // Start is called before the first frame update
    public override void Initialize()
    {
        base.Initialize();
        lowerBound = 2;
        upperBound = 4;
        
        Speed = 2;

        description = "Deals 2x damage to crystals.";

        myName = "Excavate";
        CardType = CardType.MeleeAttack;
        Renderer renderer = GetComponent<Renderer>();
    }

    public override void CardIsUnstaggered()
    {
        base.CardIsUnstaggered();
        if (Origin.HasAnimationParameter("IsAttacking"))
        {
            Origin.AttackAnimation("IsAttacking");
        }
    }

    // does 2x damage if target is crystal
    public override void OnHit()
    {
        AudioManager.Instance.PlaySFX(EXCAVATE_SOUND_EFFECT_NAME);
        if (Target is Crystals)
        {
            if (Origin.HasAnimationParameter("IsAttacking"))
            {
                Origin.AttackAnimation("IsAttacking");
            }
            Vector3 diffInLocation = Target.myTransform.position - Origin.myTransform.position;
            Origin.UpdateFacing(diffInLocation, null);
            this.Target.TakeDamage(Origin, 2 * rolledCardStats.actualRoll);
        }
        else
        {
            base.OnHit();
        }
    }
}
