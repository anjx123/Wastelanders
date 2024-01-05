using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public override void Start()
    {
        base.Start();
        combatInfo.FaceLeft();
        CombatManager.Instance.enemies.Add(this);
    }

    /*  Given a list of players, the enemy chooses appropriately a target/targets and adds an attack that it chooses to the bq.
     *  It is up to the subclass HOW to implement this method, as certain enemies may prefer to attack certain players. 
     *  
     */
    public abstract void AddAttack(List<PlayerClass> players);

    public override void FaceRight()
    {
        this.GetComponent<SpriteRenderer>().flipX = true;
        combatInfo.FaceRight();
    }

    public override void FaceLeft()
    {
        this.GetComponent<SpriteRenderer>().flipX = false;
        combatInfo.FaceLeft();
    }
}
