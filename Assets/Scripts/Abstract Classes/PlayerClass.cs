using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerClass : EntityClass
{

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
    abstract protected void DrawCard();

    protected abstract void RenderHand();

    /*  Called by HighlightManager whenever an action is declared. Deletes the used card.
     *  REQUIRES: Nothing
     *  MODIFIES: hand, discard
     */
    abstract public void HandleUseCard(ActionClass a);

    public override IEnumerator ResetPosition()
    {
        yield return StartCoroutine(MoveToPosition(initalPosition, 0f, 0.8f));
        FaceRight();
    }

    public override IEnumerator Die()
    {
        yield return StartCoroutine(MoveToPosition(myTransform.position + new Vector3(-10, 0, 0), 0, 0.8f));
        CombatManager.Instance.RemovePlayer(this);
        isDead = true;
        this.gameObject.SetActive(false);
    }

    public override void FaceRight()
    {
        this.GetComponent<SpriteRenderer>().flipX = false;
        combatInfo.FaceRight();
    }

    public override void FaceLeft()
    {
        this.GetComponent<SpriteRenderer>().flipX = true;
        combatInfo.FaceLeft();
    }

    public void DrawToMax()
    {
        for (int i = hand.Count; i < maxHandSize; i++)
        {
            DrawCard();
        }
        RenderHand();
        
    }
}
