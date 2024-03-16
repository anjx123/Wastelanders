using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public abstract class SlimeAttacks : ActionClass
{
#nullable enable
    protected bool onHitWasCalled = false;
    protected delegate void AttackCallback();

    public override void CardIsUnstaggered()
    {
        if (!onHitWasCalled)
        {
            StartCoroutine(AttackAnimation(null));
        }
        onHitWasCalled=false;
    }

    protected abstract IEnumerator AttackAnimation(AttackCallback? attackCallback);
}
