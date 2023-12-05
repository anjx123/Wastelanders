using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClass : EntityClass
{
    public List<GameObject> cardPrefabs;

    private int maxHandSize = 5;

    // This is the deck of the player, which does not change as the player reshuffles/draws cards.
    private List<GameObject> deck = new List<GameObject>();

    // This is the pool, which is initialized to the deck. Then draws will remove cards from the pool.
    private List<GameObject> pool = new List<GameObject>();

    // Drawn cards move from pool to hand
    private List<GameObject> hand = new List<GameObject>();

    // technically not necessaary as of now, but in case we want to do stuff with used cards (or single use)
    private List<GameObject> discard = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < cardPrefabs.Count; i++)
        {
            GameObject toAdd = Instantiate(cardPrefabs[i]);
            deck.Add(toAdd);
        }
        pool = deck;
        for (int i = 0; i < maxHandSize; i++)
        {
            DrawCard();
        }
    }


    /*  Draws a single card from the pool, removing it from the pool and refilling it if necessary.
     *  REQUIRES: Nothing
     *  MODIFIES: pool, hand, maxHandSize
     *  
     */
    void DrawCard()
    {
        if (pool.Count == 0)
        {
            maxHandSize--;
            pool = deck;
        }
        int idx = Random.Range(0, pool.Count - 1);
        hand.Add(pool[idx]);
        pool.RemoveAt(idx);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
