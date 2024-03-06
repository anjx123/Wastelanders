using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class QueenBeetle : EnemyClass
{

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        MaxHealth = 30;
        Health = MaxHealth;
        myName = "The Queen";

        Beetle.OnGainBuffs += HandleGainedBuffs;
    }

    // event handler for Beetle.OnGainBuffs. This is called whenever a beetle tries to
    // add a buff.
    // Adds the stacks that were directed to the beetle to the Queen instead.
    private void HandleGainedBuffs(string buffType, int stacks)
    {
        AddStacks(buffType, stacks);
        Debug.Log("Queen received " + stacks + " stacks of " + buffType + ".");
    }
}
