
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FistCards : ActionClass
{
    public override sealed void Start()
    {
        //No initialization code here
    }

    public override void Initialize()
    {
        CardType = CardType.MeleeAttack;
        base.Initialize();
    }
    public override void OnHit()
    {
        //Origin.AttackAnimation("IsFisting(?)");

        base.OnHit();
    }
}
