using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class WasteFrog : EntityClass
{
    
    List<ActionClass> availableActions;
    


    // Start is called before the first frame update
    public override void Start()
    {
        
        MAX_HEALTH = 15;
        health = MAX_HEALTH;
        animator = GetComponent<Animator>();
        myTransform = GetComponent<Transform>();
        myName = "Le Frog";
        Debug.Log(myName + " is ready for combat!");
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of frog
        base.Start();

    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        //StartCoroutine(StaggerBack(myTransform.position + new Vector3(1.5f, 0, 0)));
    }



    

    // Update is called once per frame
    void Update()
    {
        
    }
}
