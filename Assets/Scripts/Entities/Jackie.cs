using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jackie : PlayerClass
{
    public List<GameObject> cardPrefabs;
    public RectTransform handContainer;
    public int cardWidth;

    // Start is called before the first frame update
    public override void Start()
    {
        MAX_HEALTH = 30;
        health = MAX_HEALTH;
        myName = "Jackie";

        HighlightManager.player = this;

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

        // Draw starting hand
        for (int i = 0; i < maxHandSize; i++)
        {
            DrawCard();
        }

        RenderHand();
        Debug.Log(myName + " is ready for combat!");

        base.Start();
    }

    override protected void DrawCard()
    {
        if (pool.Count == 0)
        {
            maxHandSize--;
            for (int i = 0; i < discard.Count; i++)
            {
                pool.Add(discard[i]);
            }
            discard.Clear();
            Debug.Log("Reshuffling deck. New max hand size is " + maxHandSize);
        }

        if (hand.Count < maxHandSize)
        {
            int idx = Random.Range(0, pool.Count);
            hand.Add(pool[idx]);
            pool.RemoveAt(idx);
        }
    }

    /*  Renders the cards in List<GameObject> hand to the screen, as children of the handContainer.
     *  Cards are filled in left to right.
     *  REQUIRES: Nothing
     *  MODIFIES: Nothing
     * 
     */
    void RenderHand()
    {
        for (int i = 0; i < hand.Count; i++)
        {
            hand[i].transform.SetParent(handContainer.transform, false);
            hand[i].transform.position = Vector3.zero;

            float distanceToLeft = (float)(handContainer.rect.width / 2 - (i * cardWidth * 0.9));

            float y = handContainer.transform.position.y;
            Vector3 v = new Vector3(-distanceToLeft, y, i + 10);
            hand[i].transform.position = v;
            hand[i].transform.rotation = Quaternion.Euler(0, 0, -5);
        }
    }

    /*  Called by HighlightManager whenever an action is declared. Deletes the used card.
     *  REQUIRES: Nothing
     *  MODIFIES: hand, discard
     */
    override public void HandleUseCard(ActionClass a)
    {
        // remove the used card
        GameObject used = FindChildWithScript(handContainer.gameObject, a.GetType());
        hand.Remove(used);
        discard.Add(used);
        used.transform.position = new Vector3(500, 500, 1);
        // draw a replacement card
        DrawCard();
        RenderHand();
    }

    /* helper function for HandleUseCard, it takes a gameobject and type and returns
     * a child gameobject that has a script of that type.
     * 
     */
    GameObject FindChildWithScript(GameObject parent, System.Type type)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            Transform child = parent.transform.GetChild(i);
            ActionClass[] components = child.gameObject.GetComponents<ActionClass>();
            if (components[0].GetType() == type)
            {
                return child.gameObject;
            }
        }

        return null;
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(5);
        }        
    }
}
