using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public abstract class PlayerClass : EntityClass
{
    public List<GameObject> cardPrefabs;
    public RectTransform handContainer;
    public int cardWidth;
#nullable enable

    public delegate void PlayerEventDelegate(PlayerClass player);
    public static event PlayerEventDelegate? playerReshuffleDeck;


    // for manual removal from the battle queue in case the player changes their mind. 
    public void ReaddCard(ActionClass card)
    {
        hand.Add(card.gameObject);
        discard.Remove(card.gameObject);
        HighlightManager.RenderHandIfAppropriate(this);
    }

    protected int maxHandSize = 4;
    // This is the deck of the player, which does not change as the player reshuffles/draws cards.
    protected List<GameObject> deck = new List<GameObject>();

    // This is the pool, which is initialized to the deck. Then draws will remove cards from the pool.
    protected List<GameObject> pool = new List<GameObject>();

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

    // "unrenders" the hand or more explictly:
    // " Moves the player's cards off-screen to hide them.
    // Note that this doesn't disable the card objects. "

    public void UnRenderHand()
    {
        for (int i = 0; i < hand.Count; i++)
        {
            hand[i].transform.position = new Vector3(-10, -10, -10);
        }
    }

    /*  Renders the cards in List<GameObject> hand to the screen, as children of the handContainer.
    *  Cards are filled in left to right.
    *  REQUIRES: Nothing
    *  MODIFIES: Nothing
    * 
    */
    public void RenderHand()
    {
        for (int i = 0; i < hand.Count; i++)
        {
            hand[i].transform.SetParent(handContainer.transform, false);
            hand[i].transform.position = Vector3.zero;

            float distanceToLeft = (float)(handContainer.rect.width / 2 - (i * cardWidth));

            float y = handContainer.transform.position.y;
            Vector3 v = new Vector3(-distanceToLeft, y, -i);
            hand[i].transform.position = v;
            hand[i].transform.rotation = Quaternion.Euler(0, 0, -5);
        }
        RenderText();
    }

    // Renders the information (text) of each card inside the player's hand. 
    protected void RenderText()
    {
        for (int i = 0; i < hand.Count; i++)
        {
            hand[i].GetComponent<ActionClass>().UpdateDup();
        }
    }

/*    Called by HighlightManager whenever an action is declared i.e.selected.Deletes the used card.

   * REQUIRES: Nothing
   * MODIFIES: hand, discard
*/

    public void HandleUseCard(ActionClass a)
    {
        // remove the used card
        GameObject used = FindChildWithScript(handContainer.gameObject, a.GetType());
        hand.Remove(used);
        discard.Add(used);
        used.transform.position = new Vector3(500, 500, 1); // spirit the action away; Note: this works because if the action is readded to the deck, the RenderHand() method effectively spirits it back.
        // draw a replacement card; TODO: when?
        // RenderHand(); 
        HighlightManager.RenderHandIfAppropriate(this);
    }

/*    helper function for HandleUseCard, it takes a gameobject and type and returns
    * a child gameobject that has a script of that type.*/

    GameObject FindChildWithScript(GameObject parent, System.Type type)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            Transform child = parent.transform.GetChild(i);
            ActionClass[] components = child.gameObject.GetComponents<ActionClass>();
            // Debug.Log(components[0].GetType());
            if (components[0].GetType() == type)
            {
                return child.gameObject;
            }
        }
        return null;
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
