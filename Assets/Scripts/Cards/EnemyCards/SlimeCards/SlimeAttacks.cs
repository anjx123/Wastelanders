using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public abstract class SlimeAttacks : ActionClass
{
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
