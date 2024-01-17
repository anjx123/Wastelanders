using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleQueueIcons : SelectClass
{
    public ActionClass actionClass; // Current/last ActionClass that we are displaying; it is set by the enemy
    public GameObject cardDisplay; // The card display object
    public bool isDisplaying = false;
    SpriteRenderer rdr;
    public static BattleQueueIcons currentUser; // set this to self when we display; this way,
                                                // other instances can turn off our flag when they overwrite us
    private bool targetHighlighted = false;

    public override void OnMouseDown()
    {
        if (actionClass.Origin is PlayerClass) {
                DeleteFromBQ();
        } else {
            if (isDisplaying)
            {
                // If the card is currently displaying, hide it
                HideCard();
            }
            else
            {
                // If the card is not currently displaying, show it
                ShowCard();
            }
        }
    }

    private void DeleteFromBQ()
    {
        BattleQueue.BattleQueueInstance.DeletePlayerAction(actionClass);
    }

    private void ShowCard()
    {
        if (CombatCardUI.currentUser != null)
        {
            CombatCardUI.currentUser.isDisplaying = false;
            CombatCardUI.currentUser.DeHighlightTarget();
            CombatCardUI.currentUser = null;
        }
        if (currentUser != null)
        {
            currentUser.isDisplaying = false;
            currentUser.DeHighlightTarget();
        }
        if (cardDisplay.GetComponent<SpriteRenderer>() == null)
        {
            cardDisplay.AddComponent<SpriteRenderer>();
        }
        rdr = cardDisplay.GetComponent<SpriteRenderer>();
        rdr.sprite = actionClass.fullCard;
        isDisplaying = true;
        currentUser = this;
        HighlightTarget();
    }

    private void HighlightTarget()
    {
        if (!targetHighlighted)
        {
            actionClass.Target.OnMouseEnter();
        }
        targetHighlighted = true;
    }

    public void DeHighlightTarget()
    {
        actionClass.Target.OnMouseExit();
        targetHighlighted = false;
    }

    private void HideCard()
    {
        Destroy(rdr);
        isDisplaying = false;
    }

    public override void OnMouseEnter()
    {
        // Increase the size of the Combat UI to indicate it's clickable
        transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        HighlightTarget();
    }

    public override void OnMouseExit()
    {
        // Reset the size when the mouse is no longer over the Combat UI
        transform.localScale = new Vector3((float)0.2, (float)0.2, 1);
        DeHighlightTarget();
    }
}
