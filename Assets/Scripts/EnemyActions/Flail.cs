using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flail : ActionClass
{
	public Flail()
	{
	}

    public override void ExecuteActionEffect()
    {
        throw new System.NotImplementedException();
    }

    public void Start()
    {
        Speed = 2;
        Block = 4;
        Damage = 3;
        myName = "Flail";
        CardType = CardType.MeleeAttack;
    }
}

