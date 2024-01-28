using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StaffCards : ActionClass
{
    // Start is called before the first frame update
    public virtual void Start()
    {
        CardType = CardType.MeleeAttack;  
    }

    public override void OnHit()
    {
        Origin.AttackAnimation("IsStaffing");
        base.OnHit();
    }
}
