using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StaffCards : ActionClass
{
    public override sealed void Start()
    {
        //No initialization code here
    }
    public override void Initialize()
    {
        base.Initialize();
    }

    public override void CardIsUnstaggered()
    {
        if (Origin.HasAnimationParameter("IsStaffing"))
        {
            Origin.AttackAnimation("IsStaffing");
        } else
        {
            Origin.AttackAnimation("MeleeAttack");
        }
        base.CardIsUnstaggered();
    }
    public override void OnHit()
    {
        Origin.AttackAnimation("IsStaffing");
        base.OnHit();
    }
}
