using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WasteFrog : EntityClass
{
    List<ActionClass> availableActions;

    // Start is called before the first frame update
    void Start()
    {
        MAX_HEALTH = 15;
        health = MAX_HEALTH;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
