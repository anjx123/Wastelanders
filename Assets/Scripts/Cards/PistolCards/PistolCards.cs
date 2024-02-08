using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PistolCards : ActionClass
{
    public override void Start()
    {
        base.Start();
        CardType = CardType.RangedAttack;
    }
    public override void OnHit()
    {
        Origin.AttackAnimation("IsShooting");
        base.OnHit();
    }
}
