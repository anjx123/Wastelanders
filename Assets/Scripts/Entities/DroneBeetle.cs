using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneBeetle : EnemyClass
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        MaxHealth = 7;
        Health = MaxHealth;
        myName = "Drone";
    }
}
