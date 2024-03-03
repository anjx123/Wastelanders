using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerBeetle : BeetleMinions
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        MaxHealth = 8;
        Health = MaxHealth;
        myName = "Worker";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
