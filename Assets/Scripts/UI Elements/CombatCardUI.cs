using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using static ActionClass;

public class CombatCardUI : DisplayableClass
{
    [SerializeField] SpriteRenderer targetRenderer;
    [SerializeField] TextMeshPro rangeText;

    [SerializeField] SpriteRenderer oneTimeUseBuff;
    [SerializeField] TextMeshPro buffIncreaseText;

    [SerializeField] GameObject oneTimeBuffObj;
    [SerializeField] GameObject buffFlipPreserver;
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
            ActionClass.CardValuesUpdating -= UpdateRangeText;
        }
    }

    private void OnEnable()
    {
        rangeText.GetComponent<MeshRenderer>().sortingLayerName = targetRenderer.sortingLayerName;
        rangeText.GetComponent<MeshRenderer>().sortingOrder = targetRenderer.sortingOrder;

        buffIncreaseText.GetComponent<MeshRenderer>().sortingLayerName = targetRenderer.sortingLayerName;
        buffIncreaseText.GetComponent<MeshRenderer>().sortingOrder = targetRenderer.sortingOrder;
    }

    public void FaceRight()
    {/*
        FlipTransform(oneTimeBuffObj.transform, true);
        FlipTransform(buffFlipPreserver.transform, true);*/
    }

    public void FaceLeft()
    {/*
        FlipTransform(oneTimeBuffObj.transform, false);
        FlipTransform(buffFlipPreserver.transform, false);*/
    }

    public void SetBuffIcon(ActionClass.CardDup cardDup)
    {
        (string buffName, int lowerBound, int upperBound) = cardDup.oneTimeBuffs;
        if (lowerBound > 0 || upperBound > 0)
        {
            buffIncreaseText.text = "+" + lowerBound + "-" + upperBound;
            oneTimeUseBuff.sprite = Resources.Load<Sprite>("StatusIcon/" + buffName);
        } else
        {
            buffIncreaseText.text = "";
            oneTimeUseBuff.sprite = null;
        }
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
        (string buffName, int buffLowerBound, int buffUpperBound) = actionClass.GetCard().oneTimeBuffs;
        rangeText.text = (actionClass.GetCard().rollFloor - buffLowerBound) + "-" + (actionClass.GetCard().rollCeiling - buffUpperBound);
        if (ActionClass.Origin is EnemyClass)
        {
            rangeText.color = Color.red;
        }
        else
        {
            rangeText.color = Color.green;
        }

        SetBuffIcon(actionClass.GetCard());
    }

    public void Emphasize()
    {

        GetComponent<SpriteRenderer>().sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER + 1;

        rangeText.GetComponent<MeshRenderer>().sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER + 2;
        targetRenderer.GetComponent<SpriteRenderer>().sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER + 2;

        oneTimeUseBuff.sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER + 2;
        buffIncreaseText.GetComponent<MeshRenderer>().sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER + 2;

    }

    public void DeEmphasize()
    {
        GetComponent<SpriteRenderer>().sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER - 2;

        rangeText.GetComponent<MeshRenderer>().sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER - 1;
        targetRenderer.GetComponent<SpriteRenderer>().sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER - 1;


        oneTimeUseBuff.sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER - 1;
        buffIncreaseText.GetComponent<MeshRenderer>().sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER - 1;
    }

    private void SetTargetIcon(ActionClass actionClass)
    {
        targetRenderer.sprite = actionClass.Target.icon;
    }

    public void FlipTransform(Transform transform, bool faceRight)
    {
        if (faceRight) //Face Right
        {
            Vector3 flippedTransform = transform.localScale;
            flippedTransform.x = Mathf.Abs(flippedTransform.x);
            transform.localScale = flippedTransform;
        }
        else
        {
            Vector3 flippedTransform = transform.localScale;
            flippedTransform.x = -Mathf.Abs(flippedTransform.x);
            transform.localScale = flippedTransform;
        }
    }
}
