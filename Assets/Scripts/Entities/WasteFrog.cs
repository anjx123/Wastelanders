using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class WasteFrog : EnemyClass
{
    
    public List<GameObject> availableActions;
    


    // Start is called before the first frame update
    public override void Start()
    {
        
        MAX_HEALTH = 15;
        health = MAX_HEALTH;
        animator = GetComponent<Animator>();
        myTransform = GetComponent<Transform>();
        myName = "Le Frog";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of frog
        deck = availableActions;
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

    public override void AddAttack()
    {
        if (pool.Count < 1)
        {
            for (int i = 0; i < deck.Count; i++)
            {
                pool.Add(deck[i]);
            }
        }
        int idx = Random.Range(0, pool.Count);
        BattleQueue.BattleQueueInstance.AddEnemyAction(pool[idx].GetComponent<ActionClass>(), this);
        Debug.Log("I would have played: " + pool[idx].name);
        pool.RemoveAt(idx);
    }
}
