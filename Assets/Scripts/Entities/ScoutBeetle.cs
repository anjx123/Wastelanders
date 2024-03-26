using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ScoutBeetle : Beetle
{


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        MaxHealth = 8;
        Health = MaxHealth;
        myName = "Scout Beetle";
    }

    // Scout attacks twice in a turn
    public override void AddAttack(List<PlayerClass> players)
    {
        base.AddAttack(players);
        base.AddAttack(players);
    }
}
