using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyClass : EntityClass
{
    // NOTE: ENEMIES USE LISTS OF ACTIONCLASSES BECAUSE WE DONT HAVE TO SHOW THEM AS CARDS ON UI

    // This is the deck of the enemy, which does not change as they reshuffle/play cards.
    protected List<ActionClass> deck = new List<ActionClass>();

    // This is the pool, which is initialized to the deck. Then cards will be taken from here.
    protected List<ActionClass> pool = new List<ActionClass>();

    // technically not necessaary as of now, but in case we want to do stuff with used cards (or single use)
    protected List<ActionClass> discard = new List<ActionClass>();

    /*  Plays a single card from the pool, removing it from the pool and refilling it if necessary.
     *  REQUIRES: Nothing
     *  MODIFIES: pool
     *  
     */
    public void AddAttack()
    {
        if (pool.Count < 1)
        {
            for (int i = 0; i < deck.Count; i++)
            {
                pool.Add(deck[i]);
            }
        }
        int idx = Random.Range(0, pool.Count);
        BattleQueue.BattleQueueInstance.AddEnemyAction(pool[idx], this);
        Debug.Log("I would have played: " + pool[idx].name);
        pool.RemoveAt(idx);
    }

}
