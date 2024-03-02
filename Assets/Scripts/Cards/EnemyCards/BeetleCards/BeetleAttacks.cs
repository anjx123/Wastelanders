using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BeetleAttacks : ActionClass
{
    // Start is called before the first frame update
    public override sealed void Start()
    {
        
    }

    public override void Initialize()
    {
        base.Initialize();
        CardType = CardType.MeleeAttack;
    }

    public override void OnHit()
    {
        base.OnHit();
    }
}
