using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ActionClass;

public class CardUI : MonoBehaviour
{
    [SerializeField] private GameObject selectedForDeckUI;

    //Will activate the checkmark on card UI for indication that it is in the player's deck
    public void SetSelectedForDeck(bool isSelectedForDeck)
    {
        selectedForDeckUI.SetActive(isSelectedForDeck);
    }
}
