using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PistolCards : ActionClass
{
    public override sealed void Start()
    {
        //No initialization code here
    }

    public override void Initialize()
    {
        CardType = CardType.RangedAttack;
        base.Initialize();
    }

    public override void CardIsUnstaggered()
    {
        if (Origin.HasAnimationParameter("IsShooting"))
        {
            Origin.AttackAnimation("IsShooting");
        } else
        {
            Origin.AttackAnimation("RangedAttack");
        }
        base.CardIsUnstaggered();
    }
    public override void OnHit()
    {
        Origin.AttackAnimation("IsShooting");
        MusicManager.Instance.PlaySFX(MusicManager.SFXList.pistol);// granted you wouldn't want a pew for rapid fire...
        base.OnHit();
    }
}
