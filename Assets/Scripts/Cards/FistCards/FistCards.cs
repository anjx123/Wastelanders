
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FistCards : ActionClass
{
    public override void CardIsUnstaggered()
    {
        Origin.AttackAnimation("IsMelee");
        base.CardIsUnstaggered();
    }

    public override void OnHit()
    {
        AudioManager.Instance?.PlaySFX(AudioManager.SFXList.FIST);
        Origin.AttackAnimation("IsMelee");
        base.OnHit();
    }
}
