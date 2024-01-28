using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public abstract class FrogAttacks : ActionClass
{
    //SETUP CODE GOES IN AWAKE()
    public void Start()
    {
        //NO CODE HERE
    }

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
