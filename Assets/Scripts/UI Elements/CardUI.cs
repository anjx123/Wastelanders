using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static ActionClass;

public class CardUI : MonoBehaviour
{
    [SerializeField] private GameObject selectedForDeckUI;
    [SerializeField] private TextMeshProUGUI NameText;
    [SerializeField] private TextMeshProUGUI lowerBoundText;
    [SerializeField] private TextMeshProUGUI upperBoundText;
    [SerializeField] private TextMeshProUGUI SpeedText;
    [SerializeField] private Canvas textCanvas;
    [SerializeField] private SpriteRenderer iconRenderer;
    [SerializeField] private SpriteRenderer cardBackRenderer;
    [SerializeField] private SpriteRenderer damageIconRenderer;
    [SerializeField] private SpriteRenderer lockRenderer;
    [SerializeField] private Sprite defenseIcon;
    [SerializeField] private Sprite damageIcon;
    [SerializeField] private TMP_Text cardCost;    


    //Will activate the checkmark on card UI for indication that it is in the player's deck
    public void SetSelectedForDeck(bool isSelectedForDeck)
    {
        selectedForDeckUI?.SetActive(isSelectedForDeck);
    }

    public bool shouldRenderCost = false;
    public void RenderCard(ActionClass actionClass)
    {
        textCanvas.overrideSorting = true; //Added so it overrides the layer of its parent canvas
        ActionClass.RolledStats duplicateCard = actionClass.GetRolledStats();
        NameText.text = actionClass.GetName();
        lowerBoundText.text = duplicateCard.rollFloor.ToString();
        upperBoundText.text = duplicateCard.rollCeiling.ToString();
        SpeedText.text = actionClass.Speed.ToString();
        iconRenderer.sprite = actionClass.GetIcon();

        // Render the card's back depending on the evolution state
        bool displayFlipped = actionClass.IsFlipped || (actionClass.IsEvolved && actionClass.CanEvolve());
        cardBackRenderer.sprite = displayFlipped ? actionClass.evolvedCardBack : actionClass.cardBack;
        lockRenderer.enabled = !actionClass.CanEvolve() && (actionClass.IsFlipped || actionClass.IsEvolved);

        if (actionClass.CardType == CardType.Defense)
        {
            damageIconRenderer.sprite = defenseIcon;
        } else
        {
            damageIconRenderer.sprite = damageIcon;
        }

        if (shouldRenderCost)
        {
            cardCost.gameObject.SetActive(true); 
            cardCost.text = actionClass.CostToAddToDeck.ToString();
            cardCost.gameObject.GetComponent<MeshRenderer>().sortingOrder = cardBackRenderer.sortingOrder + 1;
            cardCost.gameObject.GetComponent<MeshRenderer>().sortingLayerID = cardBackRenderer.sortingLayerID;
        } else
        {
            cardCost.text = "";
            cardCost.gameObject.SetActive(false);
        }

        // Now update colors
        if (duplicateCard.rollFloor > actionClass.LowerBound)
        {
            lowerBoundText.color = Color.green;
        }

        if (duplicateCard.rollCeiling > actionClass.UpperBound)
        {
            upperBoundText.color = Color.green;
        }

        if (duplicateCard.rollFloor < actionClass.LowerBound)
        {
            lowerBoundText.color = Color.red;
        }

        if (duplicateCard.rollCeiling < actionClass.UpperBound)
        {
            upperBoundText.color = Color.red;
        }

        if (duplicateCard.rollFloor == actionClass.LowerBound)
        {
            lowerBoundText.color = Color.black;
        }

        if (duplicateCard.rollCeiling == actionClass.UpperBound)
        {
            upperBoundText.color = Color.black;
        }

        lockRenderer.enabled = actionClass.IsFlipped && !actionClass.CanEvolve();
    }
}
