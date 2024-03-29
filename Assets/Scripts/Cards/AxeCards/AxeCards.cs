using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AxeCards : ActionClass
{
    public override sealed void Start()
    {
        //No initialization code here
    }


    public override void CardIsUnstaggered()
    {
        if (Origin.HasAnimationParameter("IsAxing")) // TODO: update
        {
            Origin.AttackAnimation("IsAxing"); // TODO: update
        } else
        {
            Origin.AttackAnimation("MeleeAttack"); // TODO: update
        }
        base.CardIsUnstaggered();
    }
    public override void OnHit()
    {
        if (Origin.HasAnimationParameter("IsAxing")) // TODO: update
        {
            Origin.AttackAnimation("IsAxing"); // TODO: update
        }
        else
        {
            Origin.AttackAnimation("MeleeAttack"); // TODO: update
        }
        base.OnHit();
    }
}
