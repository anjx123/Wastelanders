
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FistCards : ActionClass
{
    public const string FIST_ANIMATION_NAME = "IsPunching";
    public const string FIST_SOUND_FX_NAME = "Fist Hit";
    public override void CardIsUnstaggered()
    {
        Origin.AttackAnimation(FIST_ANIMATION_NAME);
        base.CardIsUnstaggered();
    }

    public override void OnHit()
    {
        AudioManager.Instance?.PlaySFX(FIST_SOUND_FX_NAME);
        Origin.AttackAnimation(FIST_ANIMATION_NAME);
        base.OnHit();
    }
}
