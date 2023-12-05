using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClass : EntityClass
{
    public List<GameObject> cardPrefabs;
    public RectTransform handContainer;
    public int cardWidth;

    private int maxHandSize = 2;
    private int currentHandSize = 0;
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
        // initialize deck
        for (int i = 0; i < cardPrefabs.Count; i++)
        {
            GameObject toAdd = Instantiate(cardPrefabs[i]);
            deck.Add(toAdd);
            toAdd.transform.position = new Vector3(1000, 1000, 1);
        }

        // deep copy deck into pool
        for (int i = 0; i < cardPrefabs.Count; i++)
        {
            GameObject toAdd = Instantiate(deck[i]);
            pool.Add(toAdd);
            toAdd.transform.position = new Vector3(-1000, -1000, 1);
        }
        
        Debug.Log(pool.Count);
        for (int i = 0; i < maxHandSize; i++)
        {
            DrawCard();
        }

        Debug.Log(hand.Count);
        RenderHand();
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
        int idx = Random.Range(0, pool.Count);
        hand.Add(pool[idx]);
        currentHandSize++;
        pool.RemoveAt(idx);
    }

    /*
     * 
     * 
     * 
     */
    void RenderHand()
    {
        for (int i = 0; i < hand.Count; i++)
        {
            Debug.Log(i);
            hand[i].transform.SetParent(handContainer.transform, false);
            hand[i].transform.position = Vector3.zero;

            float distanceToLeft = handContainer.rect.width / 2 - (i * cardWidth);

            float y = handContainer.transform.position.y;
            Vector3 v = new Vector3(-distanceToLeft, y, 1);
            hand[i].transform.position = v;
            Debug.Log(hand[i].transform.position);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
