using System;
using System.Collections;
using System.Collections.Generic;
using Systems.Persistence;
using UnityEngine;
using WeaponDeckSerialization;

public abstract class PlayerClass : EntityClass
{
    protected List<InstantiableActionClassInfo> cardPrefabs; //Empty after intialization
#nullable enable

    public delegate void PlayerEventDelegate(PlayerClass player);
    public static event PlayerEventDelegate? playerReshuffleDeck;

    public int maxHandSize = 4;


    public List<GameObject> Hand { get { return new List<GameObject>(hand); } }

    // Drawn cards move from pool to hand
    protected List<GameObject> hand = new();
    
    // This is the pool, which is initialized to the deck. Then draws will remove cards from the pool.
    protected List<GameObject> pool = new();

    protected List<GameObject> discard = new();

    public override void Start()
    {
        if (Team == EntityTeam.NoTeam) Team = EntityTeam.PlayerTeam;
        base.Start();
        GrabDeck();
        InstantiatePool();
    }

    protected void InstantiatePool()
    {
        while (cardPrefabs.Count > 0)
        {
            int idx = UnityEngine.Random.Range(0, cardPrefabs.Count);
            GameObject toAdd = InstantiateCardPrefab(cardPrefabs[idx]);
            pool.Add(toAdd);
            toAdd.transform.position = new Vector3(-100, -100, 1);
            cardPrefabs.RemoveAt(idx);
        }
    }

    private GameObject InstantiateCardPrefab(InstantiableActionClassInfo cardInfo)
    {
        GameObject toAdd = Instantiate(cardInfo.ActionClass.gameObject);
        ActionClass card = toAdd.GetComponent<ActionClass>();
        InitializeCard(card, cardInfo);
        return toAdd;
    }

    private void InitializeCard(ActionClass card, InstantiableActionClassInfo cardInfo)
    {
        card.Origin = this;
        card.IsEvolved = cardInfo.IsEvolved;
    }

    public void InjectDeck(List<GameObject> actions)
    {
        DestroyDeck();

        // Recreate new one
        foreach (GameObject action in actions)
        {
            GameObject toAdd = Instantiate(action);
            toAdd.GetComponent<ActionClass>().Origin = this;
            pool.Add(toAdd);
            toAdd.transform.position = new Vector3(-100, -100, 1);
        }
    }

    // Adds card to player hand
    public void InjectCard(GameObject card)
    {
        GameObject toAdd = Instantiate(card);
        toAdd.GetComponent<ActionClass>().Origin = this;
        hand.Add(toAdd);
        toAdd.transform.position = new Vector3(-100, -100, 1);
    }

    public override void DestroyDeck()
    {
        List<GameObject> toDestroy = new List<GameObject>(pool);
        foreach (GameObject actionClass in toDestroy)
        {
            pool.Remove(actionClass);
            hand.Remove(actionClass);
            discard.Remove(actionClass);
            Destroy(actionClass);
        }
    }

    protected abstract void GrabDeck();
    /*  Draws a single card from the pool, removing it from the pool and refilling it if necessary.
     *  REQUIRES: Nothing
     *  MODIFIES: pool, hand, maxHandSize
     *  
     */
    protected void DrawCard()
    {
        if (pool.Count == 0)
        {
            Reshuffle();
        }

        if (hand.Count < maxHandSize)
        {
            int idx = 0;
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

    protected void Reshuffle()
    {
        playerReshuffleDeck?.Invoke(this);
        maxHandSize--;

        while (discard.Count > 0)
        {
            int idx = UnityEngine.Random.Range(0, discard.Count);
            pool.Add(discard[idx]);
            discard.RemoveAt(idx);
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

    public override void PerformSelection()
    {
        DrawToMax();
        StartCoroutine(ResetPosition());
    }

    public override IEnumerator ResetPosition()
    {
        yield return StartCoroutine(MoveToPosition(initialPosition, 0f, 0.8f));
        FaceOpponent();
    }

    public void DrawToMax()
    {
        for (int i = hand.Count; i < maxHandSize; i++)
        {
            DrawCard();
        }
    }
}
