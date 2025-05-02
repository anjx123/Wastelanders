using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AxeCards : ActionClass
{
    public const string AXE_ANIMATION_NAME = "IsAxing";
    public const string AXE_SOUND_FX_NAME = "Axe Cut";
    public override sealed void Start()
    {
        //No initialization code here
    }


    public override void CardIsUnstaggered()
    {
        if (Origin.HasAnimationParameter(AXE_ANIMATION_NAME)) 
        {
            Origin.AttackAnimation(AXE_ANIMATION_NAME); 
        }
        base.CardIsUnstaggered();
    }
    public override void OnHit()
    {
        AudioManager.Instance.PlaySFX(AXE_SOUND_FX_NAME);
        if (Origin.HasAnimationParameter(AXE_ANIMATION_NAME))
        {
            Origin.AttackAnimation(AXE_ANIMATION_NAME); 
        }
        base.OnHit();
    }
}
