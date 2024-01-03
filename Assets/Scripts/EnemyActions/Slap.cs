using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slap : ActionClass
{
	public Slap()
	{
	}

    public override void ExecuteActionEffect()
    {
        throw new System.NotImplementedException();
    }

    public void Start()
    {
        Speed = 5;
        Block = 0;
        Damage = 2;
        myName = "Slap";
    }
}

