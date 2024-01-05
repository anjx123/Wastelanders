using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyClass : EntityClass
{

    // This is the deck of the enemy, which does not change as they reshuffle/play cards.
    protected List<GameObject> deck = new List<GameObject>();

    // This is the pool, which is initialized to the deck in a random order. Then cards will be taken from here. Cards are taken from index 0.
    protected List<GameObject> pool = new List<GameObject>();

    // technically not necessaary as of now, but in case we want to do stuff with used cards (or single use)
    protected List<GameObject> discard = new List<GameObject>();

    /*  Plays a single card from the pool, removing it from the pool and refilling it if necessary.
     *  REQUIRES: Nothing
     *  MODIFIES: pool
     *  
     */
    public abstract void AddAttack();

    public override void FaceLeft()
    {
        this.GetComponent<SpriteRenderer>().flipX = false;
        combatInfo.FaceLeft();
    }

    public override void FaceRight() 
    {
        this.GetComponent<SpriteRenderer>().flipX = true;
        combatInfo.FaceRight();
    }

}
