using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DroneBeetle : Beetle
{


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        MaxHealth = 7;
        Health = MaxHealth;
        myName = "DroneBeetle";
    }
}
