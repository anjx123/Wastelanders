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
            List<GameObject> temp = new List<GameObject>();
            for (int i = 0; i < deck.Count; i++)
            {
                temp.Add(deck[i]);
            }
            while (temp.Count > 0)
            {
                int idx = Random.Range(0, temp.Count);
                pool.Add(temp[idx]);
                temp.RemoveAt(idx);
            }
        }
        BattleQueue.BattleQueueInstance.AddEnemyAction(pool[0].GetComponent<ActionClass>(), this);
        Debug.Log("I would have played: " + pool[0].name);
        pool.RemoveAt(0);
    }


}
