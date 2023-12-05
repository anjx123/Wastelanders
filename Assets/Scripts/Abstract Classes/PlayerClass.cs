using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClass : EntityClass
{
    public List<GameObject> cardPrefabs;

    private int maxHandSize = 5;
    private List<ActionClass> deck;     // This is the deck of the player, which does not change as the player reshuffles/draws cards.
    private List<ActionClass> pool;     // This is the pool, which is initialized to the deck. Then draws will remove cards from the pool.
    private List<ActionClass> hand;     // Drawn cards move from pool to hand
    private List<ActionClass> discard;  // technically not necessaary as of now, but in case we want to do stuff with used cards (or single use)

    // Start is called before the first frame update
    void Start()
    {
        //GameObject card1 = Instantiate(cardPrefabs[0]);
        //deck.Add(card1);
        Debug.Log(deck);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
