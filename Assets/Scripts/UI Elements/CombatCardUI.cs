using UnityEngine;
using UnityEngine.UI;

public class CombatCardUI : Selectable
{
    public ActionClass actionClass; // The ActionClass field you mentioned
    public GameObject cardDisplay; // The card display object
    private bool isDisplaying = false; // The flag for displaying the card

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
        // Instantiate the card display object and set the flag to true
        SpriteRenderer rdr = cardDisplay.AddComponent<SpriteRenderer>();
        Debug.Log(actionClass.name);
        rdr.sprite = actionClass.fullCard;
        isDisplaying = true;
    }

    void HideCard()
    {
        Destroy(cardDisplay);
        isDisplaying = false;
    }
}
