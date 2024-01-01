using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PistolCards : ActionClass
{
    public virtual void Start()
    {
        CardType = CardType.RangedAttack;
    }
    public override void OnHit()
    {
        Origin.AttackAnimation("IsShooting");
        base.OnHit();
    }
}
