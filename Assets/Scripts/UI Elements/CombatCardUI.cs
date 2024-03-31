using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatCardUI : DisplayableClass
{
    [SerializeField] SpriteRenderer targetRenderer;
    [SerializeField] TextMeshPro rangeText;
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
    void OnDestroy()
    {
        if (ActionClass != null)
        {
            ActionClass.TargetChanged -= SetTargetIcon;
            ActionClass.CardValuesUpdating += UpdateRangeText;
        }
    }

    private void OnEnable()
    {
        rangeText.GetComponent<MeshRenderer>().sortingLayerName = targetRenderer.sortingLayerName;
        rangeText.GetComponent<MeshRenderer>().sortingOrder = targetRenderer.sortingOrder;
    }



    public void SetActionClass(ActionClass actionClass)
    {
        if (ActionClass != null)
        {
            ActionClass.TargetChanged -= SetTargetIcon;
            ActionClass.CardValuesUpdating -= UpdateRangeText;
        }
        ActionClass = actionClass;
        ActionClass.TargetChanged += SetTargetIcon;
        ActionClass.CardValuesUpdating += UpdateRangeText;
        SetTargetIcon(ActionClass);
        UpdateRangeText(ActionClass);
        GetComponent<SpriteRenderer>().sprite = actionClass.GetIcon();

    }

    void UpdateRangeText(ActionClass actionClass)
    {
        rangeText.text = actionClass.GetCard().rollFloor + "-" + actionClass.GetCard().rollCeiling;
        if (ActionClass.Origin is EnemyClass)
        {
            rangeText.color = Color.red;
        }
        else
        {
            rangeText.color = Color.green;
        }
    }

    public void Emphasize()
    {
        GetComponent<SpriteRenderer>().sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER + 1;
        rangeText.GetComponent<MeshRenderer>().sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER + 2;
        targetRenderer.GetComponent<SpriteRenderer>().sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER + 2;
    }

    public void DeEmphasize()
    {
        GetComponent<SpriteRenderer>().sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER - 2;
        rangeText.GetComponent<MeshRenderer>().sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER - 1;
        targetRenderer.GetComponent<SpriteRenderer>().sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER - 1;
    }

    private void SetTargetIcon(ActionClass actionClass)
    {
        targetRenderer.sprite = actionClass.Target.icon;
    }
}
