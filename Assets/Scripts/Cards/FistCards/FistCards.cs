
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FistCards : ActionClass
{
    protected const string ANIMATION_NAME = "IsPunching";
    public override void CardIsUnstaggered()
    {
        Origin.AttackAnimation(ANIMATION_NAME);
        base.CardIsUnstaggered();
    }

    public override void OnHit()
    {
        AudioManager.Instance?.PlaySFX(AudioManager.SFXList.FIST);
        Origin.AttackAnimation(ANIMATION_NAME);
        base.OnHit();
    }
}
