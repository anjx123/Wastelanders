using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public abstract class FrogAttacks : ActionClass
{
    public override void Initialize()
    {
        base.Initialize();
        CardType = CardType.RangedAttack;
    }
    public override void OnHit()
    {
        base.OnHit();
        Origin.AttackAnimation("IsShooting");
        
    }
}
