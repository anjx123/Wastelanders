using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AxeCards : ActionClass
{
    public override sealed void Start()
    {
        //No initialization code here
    }

    public override void Initialize()
    {
        CardType = CardType.MeleeAttack;
        base.Initialize();
    }

    public override void CardIsUnstaggered()
    {
        if (Origin.HasAnimationParameter(null)) // TODO: update
        {
            Origin.AttackAnimation(null); // TODO: update
        } else
        {
            Origin.AttackAnimation("MeleeAttack"); // TODO: update
        }
        base.CardIsUnstaggered();
    }
    public override void OnHit()
    {
        Origin.AttackAnimation(null);
        // MusicManager.Instance?.PlaySFX(MusicManager.SFXList.pistol); // TODO: update enumeration and load sound in the Editor.
        base.OnHit();
    }
}
