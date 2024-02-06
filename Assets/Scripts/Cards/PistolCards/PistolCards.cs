using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PistolCards : ActionClass
{
    public virtual void Start()
    {
        CardType = CardType.RangedAttack;
    }
    public override IEnumerator OnHit()
    {
        Origin.AttackAnimation("IsShooting");
        yield return StartCoroutine(base.OnHit());
    }
}
