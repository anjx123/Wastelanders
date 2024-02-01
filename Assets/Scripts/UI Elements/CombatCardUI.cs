using UnityEngine;
using UnityEngine.UI;

public class CombatCardUI : DisplayableClass
{

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

}
