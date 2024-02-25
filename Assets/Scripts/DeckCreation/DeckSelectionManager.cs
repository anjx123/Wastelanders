using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class DeckSelectionManager : MonoBehaviour
{
    [SerializeField] private GameObject characterSelectionUi;
    [SerializeField] private GameObject weaponSelectionUi;
    [SerializeField] private GameObject deckSelectionUi;

    private DeckSelectionState deckSelectionState;
    private DeckSelectionState DeckSelectionState
    {
        get
        {
            return deckSelectionState;
        }
        set
        {
            deckSelectionState = value;
            switch (value)
            {
                case DeckSelectionState.CharacterSelection:
                    PerformCharacterSelection();
                    break;

                case DeckSelectionState.WeaponSelection:
                    PerformWeaponSelection();
                    break;
                case DeckSelectionState.DeckSelection:
                    PerformDeckSelection();
                    break;
            }
        }
    }

    private void Start()
    {

    }

    private void OnDestroy()
    {
        
    }

    private void PerformCharacterSelection()
    {
        characterSelectionUi.SetActive(true);
        weaponSelectionUi.SetActive(false); 
        deckSelectionUi.SetActive(false);
    }

    private void PerformWeaponSelection()
    {
        characterSelectionUi.SetActive(false);
        weaponSelectionUi.SetActive(true);
        deckSelectionUi.SetActive(false);
    }

    private void PerformDeckSelection()
    {
        characterSelectionUi.SetActive(false);
        weaponSelectionUi.SetActive(false);
        deckSelectionUi.SetActive(true);
    }
}
