using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jackie : EntityClass
{


    // Start is called before the first frame update
    public override void Start()
    {
        
        MAX_HEALTH = 30;
        health = MAX_HEALTH;
        myName = "Jackie";
        Debug.Log(myName + " is ready for combat!");
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(5);
        }        
    }
}
