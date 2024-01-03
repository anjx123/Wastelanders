using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public abstract class FrogAttacks : ActionClass
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
