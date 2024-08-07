using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StaffCards : ActionClass
{
    public override sealed void Start()
    {
        //No initialization code here
    }

    public override void CardIsUnstaggered()
    {
        if (Origin.HasAnimationParameter("IsStaffing"))
        {
            Origin.AttackAnimation("IsStaffing");
        } else
        {
            Origin.AttackAnimation("IsMelee");
        }
        base.CardIsUnstaggered();
    }
    public override void OnHit()
    {
        if (Origin.HasAnimationParameter("IsStaffing"))
        {
            Origin.AttackAnimation("IsStaffing");
        }
        else
        {
            Origin.AttackAnimation("IsMelee");
        }
        MusicManager.Instance?.PlaySFX(MusicManager.SFXList.staff);
        base.OnHit();
    }
}
