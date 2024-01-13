using UnityEngine;
using UnityEngine.UI;

public class CombatCardUI : Selectable
{
    public ActionClass actionClass; // Current/last ActionClass that we are displaying; it is set by the enemy
    public GameObject cardDisplay; // The card display object
    protected bool isDisplaying = false;
    SpriteRenderer rdr;

    public static CombatCardUI currentUser; // set this to self when we display; this way,
                                            // other instances can turn off our flag when they overwrite us

    void OnMouseOver()
    {
        // Increase the size of the Combat UI to indicate it's clickable
        transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
    }

    void OnMouseExit()
    {
        // Reset the size when the mouse is no longer over the Combat UI
        transform.localScale = Vector3.one;
    }

    void OnMouseDown()
    {
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
    
    void ShowCard()
    {
        if (currentUser != null)
        {
            currentUser.isDisplaying = false;
        }

        if (cardDisplay.GetComponent<SpriteRenderer>() == null)
        {
            cardDisplay.AddComponent<SpriteRenderer>();
        }
        rdr = cardDisplay.GetComponent<SpriteRenderer>();
        rdr.sprite = actionClass.fullCard;
        isDisplaying = true;
        currentUser = this;
    }

    void HideCard()
    {
        Destroy(rdr);
        isDisplaying = false;
    }
}
