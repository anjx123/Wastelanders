using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using static CardDatabase;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using UnityEngine.AI;

public class DeckSelectionManager : MonoBehaviour
{
    [SerializeField] private GameObject characterSelectionUi;
    [SerializeField] private GameObject weaponSelectionUi;
    [SerializeField] private GameObject deckSelectionUi;
    [SerializeField] private GameObject cardArrayParent;
    [SerializeField] private CardDatabase cardDatabase;
    [SerializeField] private PlayerDatabase playerDatabase;
    private PlayerDatabase.PlayerData playerData;
    private WeaponType weaponType;
    public WeaponAmount weaponText;
    public PointsAmount pointsText;
    public static DeckSelectionManager Instance { get; private set; }
    private DeckSelectionState deckSelectionState;
    private DeckSelectionState DeckSelectionState //Might want to swap out this state machine for an event driven changing phases.
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

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null && Instance != this)
        {
            Destroy(this);
        }

    }

    public void PrevState()
    {
        if (DeckSelectionState == DeckSelectionState.WeaponSelection) {
            DeckSelectionState = DeckSelectionState.CharacterSelection;

        } else if (DeckSelectionState == DeckSelectionState.DeckSelection) {
            DeckSelectionState = DeckSelectionState.WeaponSelection;
        } else if (DeckSelectionState == DeckSelectionState.CharacterSelection) {
            SceneManager.LoadScene("LevelSelect");
        }
    }
    public void CharacterChosen(PlayerDatabase.PlayerData playerData)
    {
        this.playerData = playerData;
        DeckSelectionState = DeckSelectionState.WeaponSelection;
    }

    public void WeaponSelected(WeaponSelect c, CardDatabase.WeaponType weaponType)
    {
        if (playerData.selectedWeapons.Contains(weaponType)) {
            playerData.selectedWeapons.Remove(weaponType);
            c.SetSelected(false);
            weaponText.TextUpdate(playerData.selectedWeapons.Count.ToString() + "/2 Selected");
        } else if (playerData.selectedWeapons.Count < 2) {
            playerData.selectedWeapons.Add(weaponType);
            c.SetSelected(true);
            weaponText.TextUpdate(playerData.selectedWeapons.Count.ToString() + "/2 Selected");

        } else {
            Debug.LogWarning("Can only select 2 weapons");
        }
    }

    public void WeaponDeckEdit(CardDatabase.WeaponType weaponType)
    {
        this.weaponType = weaponType;
        RenderDecks(weaponType);
        DeckSelectionState = DeckSelectionState.DeckSelection;
    }

    public void ActionSelected(ActionClass ac)
    {
        List<SerializableWeaponListEntry> playerDeck = playerData.playerDeck;
        ActionClass pref = cardDatabase.GetCardsByType(weaponType).FirstOrDefault(action => action.GetType() == ac.GetType());
        SerializableWeaponListEntry entry = playerDeck.FirstOrDefault(entry => entry.key == weaponType);
        SerializableTuple<WeaponType, int> tupple = playerData.playerWeaponProficiency.FirstOrDefault(entry => entry.Item1 == weaponType);
        int points = 0;
        if (tupple != null) {
            points = tupple.Item2;
        } else {
            tupple = new(weaponType, 0);
            playerData.playerWeaponProficiency.Add(tupple);
        }

        if (entry != null) {
            ActionClass actionFound = entry.value.FirstOrDefault(action => action.GetType() == ac.GetType());
            if (actionFound != null) {  
                tupple.Item2 += ac.CostToAddToDeck;          
                ac.SetSelectedForDeck(false);
                entry.value.Remove(actionFound);
                int totalPoints = points + ac.CostToAddToDeck;
                pointsText.TextUpdate("Select Your Cards:\nAvailable Points: " + totalPoints.ToString());
            } else {
                if (points - ac.CostToAddToDeck >= 0) {
                    tupple.Item2 = points - ac.CostToAddToDeck;
                    ac.SetSelectedForDeck(true);
                    entry.value.Add(pref);
                    pointsText.TextUpdate("Select Your Cards:\nAvailable Points: " + tupple.Item2.ToString());
                } else {
                    Debug.LogWarning("Insufficient experience points");
                }
            }

            return;
        }

        SerializableWeaponListEntry newEntry = new()
        {
            key = weaponType,
            value = new List<ActionClass> { pref }
        };
        
        playerDeck.Add(newEntry);
    }

    

    private void Start()
    {
    }

    private void OnDestroy()
    {
        //Free up subscribed events here to prevent mem leak 
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
        weaponText.TextUpdate(playerData.selectedWeapons.Count.ToString() + "/2 Selected");
        foreach (Transform child in weaponSelectionUi.transform)
        {
            WeaponSelect deckItem = child.GetComponent<WeaponSelect>();
            if (deckItem)
            {
                deckItem.SetSelected(playerData.selectedWeapons.Contains(deckItem.type));
            }
        }
    }

    private void PerformDeckSelection()
    {
        characterSelectionUi.SetActive(false);
        weaponSelectionUi.SetActive(false);
        deckSelectionUi.SetActive(true);
    }


    //Renders the deck corresponding to (@param weaponType)
    public void RenderDecks(CardDatabase.WeaponType weaponType)
    {
        int width = 6; // The width of the grid in # of cards 
        int height = 6; // The height of the grid in # of cards 
        float xSpacing = 2.3f; 
        float ySpacing = -3.3f;
        float xOffset = -6f; //initial x Offset
        float yOffset = 1f; //initial y Offset
        float cardScaling = 0.8f;

        UnrenderDecks();

        List<ActionClass> chosenCardList = playerData.GetDeckByWeaponType(weaponType);
        List<ActionClass> cardsToRender = cardDatabase.GetCardsByType(weaponType);

        List<GameObject> instantiatedCards = new List<GameObject>();

        //In order to sort, the cards must be instantiated and initialized first :pensive:
        foreach (ActionClass card in cardsToRender)
        {
            GameObject go = Instantiate(card.gameObject);
            instantiatedCards.Add(go);
            ActionClass pref = chosenCardList.FirstOrDefault(action => action.GetType() == card.GetType());
            if (pref != null) {
                ActionClass ac = go.GetComponent<ActionClass>();
                ac.SetSelectedForDeck(true);
            }
        }

        instantiatedCards.Sort((card1, card2) => card1.GetComponent<ActionClass>().Speed.CompareTo(card2.GetComponent<ActionClass>().Speed));


        int index = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 pos = new Vector3(x * xSpacing + xOffset, y * ySpacing + yOffset, 0);

                if (index < instantiatedCards.Count)
                {
                    GameObject cardPrefab = instantiatedCards[index];

                    cardPrefab.transform.SetParent(cardArrayParent.transform);
                    cardPrefab.transform.position = pos;

                    // Scale the instance
                    Vector3 scale = cardPrefab.transform.localScale;
                    scale.x *= cardScaling;
                    scale.y *= cardScaling;
                    cardPrefab.transform.localScale = scale;

                    index++; 
                }
                else
                {
                    break;
                }
            }

            if (index >= instantiatedCards.Count)
            {
                break;
            }
        }
        SerializableTuple<WeaponType, int> tupple = playerData.playerWeaponProficiency.FirstOrDefault(entry => entry.Item1 == weaponType);
        int points = 0;
        if (tupple != null) {
            points = tupple.Item2;
        }

        pointsText.TextUpdate("Select Your Cards:\nAvailable Points: " + points);
    }

    private void UnrenderDecks()
    {
        foreach (Transform child in cardArrayParent.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
