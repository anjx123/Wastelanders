using UnityEngine;
using UnityEngine.UI;

public class CombatCardUI : DisplayableClass
{
    public GameObject cardDisplay; // The card display object
    public static CombatCardUI currentUser; // set this to self when we display; this way,
                                            // other instances can turn off our flag when they overwrite us

    private void OnMouseOver()
    {
        // Increase the size of the Combat UI to indicate it's clickable
        if (CombatManager.Instance.CanHighlight())
        {
            transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            HighlightTarget();
        }
    }

    public override void OnMouseExit()
    {
        // Reset the size when the mouse is no longer over the Combat UI
        transform.localScale = Vector3.one;
        DeHighlightTarget();
    }

    public override void OnMouseDown()
    {
            // If the card is not currently displaying, show it
            if (CombatManager.Instance.CanHighlight())
            {
                ShowCard();
            }
        
    }
    
    private void ShowCard()
    {

        CombatCardDisplayManager.Instance.ShowCard(actionClass, this);
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


}
