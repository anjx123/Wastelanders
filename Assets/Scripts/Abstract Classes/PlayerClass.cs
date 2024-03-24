using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerClass : EntityClass
{
    public List<ActionClass> cardPrefabs;
#nullable enable

    public delegate void PlayerEventDelegate(PlayerClass player);
    public static event PlayerEventDelegate? playerReshuffleDeck;

    protected int maxHandSize = 4;

    // This is the pool, which is initialized to the deck. Then draws will remove cards from the pool.
    protected List<GameObject> pool = new List<GameObject>();

    public List<GameObject> Hand { get { return new List<GameObject>(hand); } }

    // Drawn cards move from pool to hand
    protected List<GameObject> hand = new List<GameObject>();

    // technically not necessaary as of now, but in case we want to do stuff with used cards (or single use)
    protected List<GameObject> discard = new List<GameObject>();

    public override void Start()
    {
        CombatManager.Instance.AddPlayer(this);
        base.Start();
    }

    /*  Draws a single card from the pool, removing it from the pool and refilling it if necessary.
     *  REQUIRES: Nothing
     *  MODIFIES: pool, hand, maxHandSize
     *  
     */
    protected void DrawCard()
    {
        if (pool.Count == 0)
        {
            playerReshuffleDeck?.Invoke(this);
            maxHandSize--;
            for (int i = 0; i < discard.Count; i++)
            {
                pool.Add(discard[i]);
            }
            discard.Clear();
        }

        if (hand.Count < maxHandSize)
        {
            int idx = UnityEngine.Random.Range(0, pool.Count);
            if (pool.Count > 0)
            {
                hand.Add(pool[idx]);
                pool.RemoveAt(idx);
            }
            else
            {
                Debug.LogWarning(myName + "'s Pool has no cards");
            }
        }
    }
    /*    Called by HighlightManager whenever an action is declared i.e.selected.Deletes the used card.

       * REQUIRES: Nothing
       * MODIFIES: hand, discard
    */
    public void HandleUseCard(ActionClass a)
    {
        // remove the used card
        GameObject? used = a.gameObject;
        if (used == null) return;
        hand.Remove(used);
        discard.Add(used);
        used.transform.position = new Vector3(500, 500, 1); // spirit the action away; Note: this works because if the action is readded to the deck, the RenderHand() method effectively spirits it back.
        HighlightManager.Instance.RenderHandIfAppropriate(this);
    }


    // for manual removal from the battle queue in case the player changes their mind. 
    public void ReaddCard(ActionClass card)
    {
        hand.Add(card.gameObject);
        discard.Remove(card.gameObject);
        HighlightManager.Instance.RenderHandIfAppropriate(this);
    }

    public override IEnumerator ResetPosition()
    {
        yield return StartCoroutine(MoveToPosition(initialPosition, 0f, 0.8f));
        FaceRight();
    }
    //Removes entity cards and self from BQ and combat manager. Kills itself
    public override IEnumerator Die()
    {
        int runDistance = 10;
        BattleQueue.BattleQueueInstance.RemoveAllInstancesOfEntity(this);
        CombatManager.Instance.RemovePlayer(this);
        yield return StartCoroutine(MoveToPosition(myTransform.position + new Vector3(-runDistance, 0, 0), 0, 0.8f));

        isDead = true;
        this.gameObject.SetActive(false);
    }

    public void DrawToMax()
    {
        for (int i = hand.Count; i < maxHandSize; i++)
        {
            DrawCard();
        }
    }
}
