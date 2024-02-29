using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jackie : PlayerClass
{

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        MaxHealth = 30;
        Health = MaxHealth;
        myName = "Jackie";

        HighlightManager.selectedPlayer = this;

        // deep copy deck into pool
        for (int i = 0; i < cardPrefabs.Count; i++)
        {
            GameObject toAdd = Instantiate(cardPrefabs[i]);
            toAdd.GetComponent<ActionClass>().Origin = this;
            pool.Add(toAdd);
            toAdd.transform.position = new Vector3(-1000, -1000, 1);
        }

        
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
        }

        if (hand.Count < maxHandSize)
        {
            int idx = Random.Range(0, pool.Count);
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

    override public void ReaddCard(ActionClass card) {
        hand.Add(card.gameObject);
        discard.Remove(card.gameObject);
        // RenderHand(); // !!!
        HighlightManager.RenderHandIfAppropriate(this);

    }

    /*  Renders the cards in List<GameObject> hand to the screen, as children of the handContainer.
     *  Cards are filled in left to right.
     *  REQUIRES: Nothing
     *  MODIFIES: Nothing
     * 
     */
    protected override void RenderHand()
    {
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
    }

    // "unrenders" the hand
    public override void UnRenderHand()
    {
        for (int i = 0; i < hand.Count; i++)
        {
            hand[i].transform.position = new Vector3(-10, -10, 10);
        }
    }

    protected void RenderText()
    {
        for (int i = 0; i < hand.Count; i++)
        {
            hand[i].GetComponent<ActionClass>().UpdateDup();
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
        // RenderHand(); 
        HighlightManager.RenderHandIfAppropriate(this);

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

}
