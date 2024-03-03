using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenBeetle : EnemyClass
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        MaxHealth = 30;
        Health = MaxHealth;
        myName = "Queen";
        BeetleMinions.OnGainResonate += AddStacks;
    }

    private void OnDestroy()
    {
        BeetleMinions.OnGainResonate -= AddStacks;
    }
}
