using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public abstract class SlimeAttacks : ActionClass
{
    public virtual void Start()
    {
        CardType = CardType.MeleeAttack;
    }
    public override void OnHit()
    {
        base.OnHit();
        
    }
}
