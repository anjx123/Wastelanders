using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jackie : EntityClass
{
    private string myName = "Jackie";


    // Start is called before the first frame update
    void Start()
    {
        MAX_HEALTH = 30;
        health = MAX_HEALTH;
        Debug.Log(myName + " is ready for combat!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
