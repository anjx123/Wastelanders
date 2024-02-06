using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public abstract class SlimeAttacks : ActionClass
{
    public virtual void Start()
    {
        
    }

    public override void Initialize()
    {
        base.Initialize();
        CardType = CardType.MeleeAttack;
    }
    public override IEnumerator OnHit()
    {
         yield return StartCoroutine(base.OnHit());
        
    }
}
