using UnityEngine;
using UnityEngine.UI;

public class CombatCardUI : DisplayableClass
{
    [SerializeField] SpriteRenderer targetRenderer;
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

    public void SetActionClass(ActionClass actionClass)
    {
        ActionClass = actionClass;
        targetRenderer.sprite = actionClass.Target.icon;
        GetComponent<SpriteRenderer>().sprite = actionClass.GetIcon();
    }

    public void Emphasize()
    {
        GetComponent<SpriteRenderer>().sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER + 1;
        targetRenderer.GetComponent<SpriteRenderer>().sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER + 2;
    }

    public void DeEmphasize()
    {
        GetComponent<SpriteRenderer>().sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER - 2;
        targetRenderer.GetComponent<SpriteRenderer>().sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER - 1;
    }
}
