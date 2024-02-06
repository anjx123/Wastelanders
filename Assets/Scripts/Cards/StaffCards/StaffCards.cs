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

    public override IEnumerator OnHit()
    {
        Origin.AttackAnimation("IsStaffing");
        yield return StartCoroutine(base.OnHit());
    }
}
