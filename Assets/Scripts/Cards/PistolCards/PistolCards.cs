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
    public override void OnHit()
    {
        Origin.AttackAnimation("IsShooting");
        base.OnHit();
    }
}
