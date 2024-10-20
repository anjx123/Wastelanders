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
    
    // Initialized in editor
    public List<GameObject> availableActions;


    /*  Plays a single card from the pool, removing it from the pool and refilling it if necessary.
     *  REQUIRES: Nothing
     *  MODIFIES: pool
     *  
     */

    public override void Start()
    {
        base.Start();
        CombatManager.Instance.AddEnemy(this);
        InstantiateDeck();

        Reshuffle();
    }

    public virtual void InstantiateDeck()
    {
        for (int i = 0; i < availableActions.Count; i++)
        {
            GameObject toAdd = Instantiate(availableActions[i]);
            ActionClass addedClass = toAdd.GetComponent<ActionClass>();
            addedClass.Origin = this;

            deck.Add(toAdd);
        }
    }

    /*  Given a list of players, the enemy chooses appropriately a target/targets and adds an attack that it chooses to the bq.
     *  It is up to the subclass HOW to implement this method, as certain enemies may prefer to attack certain players. 
     *  
     *  Subclasses should document this method as follows:
     *  
     *  By default, assume the target is a completely random player. If this is not the case (for example, the enemy might prefer
     *  to attack some specific character), the method documentation should note this.
     *  
     *  Potentially? We might want to list the attacks available to the enemy at the beginning of combat for ease of balancing and
     *  tweaking difficulty later on.
     */
    public virtual void AddAttack(List<PlayerClass> players)
    {
        if (players.Count == 0) return;
        if (pool.Count == 0) return;
        pool[0].GetComponent<ActionClass>().Target = players[Random.Range(0, players.Count)]; // excludes the last value 
        pool[0].GetComponent<ActionClass>().Origin = this;
        BattleQueue.BattleQueueInstance.AddAction(pool[0].GetComponent<ActionClass>());
        combatInfo.AddCombatSprite(pool[0].GetComponent<ActionClass>());
        pool.RemoveAt(0);
        if (pool.Count < 1)
        {
            Reshuffle();
        }
    }

    /*  Reshuffles the deck. Should be called on start (so the enemy can display its first attack), and whenever the enemy runs out of
     *  attacks.
     *  REQUIRES: pool should be empty! But it shouldn't break anything, just mess up the enemy's order of attacks
     *  MODIFIES: pool 
     */
    protected virtual void Reshuffle()
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

    //Removes entity cards and self from BQ and combat manager. Kills itself
    public override IEnumerator Die()
    {
        int runDistance = 10;
        BattleQueue.BattleQueueInstance.RemoveAllInstancesOfEntity(this);
        CombatManager.Instance.RemoveEnemy(this);
        DestroyDeck();
        yield return StartCoroutine(MoveToPosition(myTransform.position + new Vector3(runDistance, 0, 0), 0, 0.8f));
        isDead = true;
        this.gameObject.SetActive(false);
    }

    public virtual void DestroyDeck()
    {
        foreach (GameObject card in deck)
        {
            Destroy(card);
        }
        deck.Clear();
        pool.Clear();
    }

    public override IEnumerator ResetPosition()
    {
        yield return StartCoroutine(MoveToPosition(initialPosition, 0f, 0.8f));
        FaceLeft();
    }
}
