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
    public WeaponAmount wa;
    public PointsAmount pa;
    public List<GameObject> deckPrefabs;
    public static DeckSelectionManager Instance { get; private set; }
    private DeckSelectionState deckSelectionState;
    private List<GameObject> instantiated;
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
            DestroyWeapons();
            DeckSelectionState = DeckSelectionState.CharacterSelection;

        } else if (DeckSelectionState == DeckSelectionState.DeckSelection) {
            RenderWeapons();
            DeckSelectionState = DeckSelectionState.WeaponSelection;
        } else if (DeckSelectionState == DeckSelectionState.CharacterSelection) {
            SceneManager.LoadScene("LevelSelect");
        }
    }

    private void DestroyWeapons() {
        foreach (var obj in instantiated) {
            GameObject.Destroy(obj);
        }

        instantiated.Clear();
    }

    private void RenderWeapons() 
    {

        foreach (var obj in instantiated) {
            obj.SetActive(true);
        }
    }

    public List<GameObject> GetDeck(string myName)
    {
        List<ActionClass> actions = playerDatabase.GetDeck(myName);
        List<GameObject> go = new();

        foreach (var a in actions) {
            go.Add(Instantiate(a.gameObject));
        }

        return go;
    }


    public void WeaponSelected(CharacterSelect c, CardDatabase.WeaponType weaponType)
    {
        if (playerData.weapons.Contains(weaponType)) {
            playerData.weapons.Remove(weaponType);
            c.SetSelected(false);
            wa.TextUpdate(playerData.weapons.Count.ToString() + "/2 Selected");
        } else if (playerData.weapons.Count < 2) {
            playerData.weapons.Add(weaponType);
            c.SetSelected(true);
            wa.TextUpdate(playerData.weapons.Count.ToString() + "/2 Selected");

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
                pa.TextUpdate("Select Your Cards:\nAvailable Points: " + totalPoints.ToString());
            } else {
                if (points - ac.CostToAddToDeck >= 0) {
                    tupple.Item2 = points - ac.CostToAddToDeck;
                    ac.SetSelectedForDeck(true);
                    entry.value.Add(pref);
                    pa.TextUpdate("Select Your Cards:\nAvailable Points: " + tupple.Item2.ToString());
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

    public void CharacterChosen(PlayerDatabase.PlayerData playerData) 
    {
        this.playerData = playerData;
        DeckSelectionState = DeckSelectionState.WeaponSelection;
        bool rendered1 = false;
        float xOffset = -3f;
        instantiated.Clear();
        
        foreach (var deckPrefab in deckPrefabs)
        {
            string name = deckPrefab.name;
            string removeDeck = name[..^4].ToUpper();

            foreach (var tuple in playerData.playerWeaponProficiency)
            {
                if (tuple.Item1.ToString() == removeDeck)
                {
                    GameObject deck = Instantiate(deckPrefab);
                    CharacterSelect c = deck.GetComponent<CharacterSelect>();
                    if (Enum.TryParse(removeDeck, out WeaponType weapT) && playerData.weapons.Contains(weapT)) {
                        c.SetSelected(true);
                    }
                    
                    instantiated.Add(deck);
                    Vector3 newPosition = deck.transform.position; // Get initial position of the deck
            
                    if (!rendered1) {
                        deck.transform.position = newPosition + new Vector3(xOffset, 0f, 0f); // Position the first deck
                        rendered1 = true;
                        xOffset += 6f; // Adjust the x offset for the second deck
                    } else {
                        deck.transform.position = newPosition + new Vector3(xOffset, 0f, 0f); // Position the second deck
                    }
                    break;
                }
            }
        }

         wa.TextUpdate(playerData.weapons.Count.ToString() + "/2 Selected");

    }

    private void Start()
    {
        instantiated = new List<GameObject>();
    //    RenderDecks(CardDatabase.WeaponType.STAFF); //Simply for testing, remove once broadcasting weapon type is hooked up
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
    }

    private void SetInactiveWeapons()
    {
        foreach (GameObject obj in instantiated)
        {
            if (obj != null)
            {
                obj.SetActive(false); // De-instantiate each GameObject
            }
        }
    }

    private void PerformDeckSelection()
    {
        SetInactiveWeapons();
        characterSelectionUi.SetActive(false);
        weaponSelectionUi.SetActive(false);
        deckSelectionUi.SetActive(true);
    }

    private List<ActionClass> GetChosenCards(CardDatabase.WeaponType weaponType) {
        foreach (var entry in playerData.playerDeck)
        {
            // Check if the current entry's key matches the keyToCheck
            if (entry.key == weaponType)
            {
                return entry.value;
            }
        }

        return new List<ActionClass>();
    }

    //Renders the deck corresponding to (@param weaponType)
    public void RenderDecks(CardDatabase.WeaponType weaponType)
    {
        List<ActionClass> chosenCardList = GetChosenCards(weaponType);
        int width = 6; // The width of the grid in # of cards 
        int height = 6; // The height of the grid in # of cards 
        float xSpacing = 2.3f; 
        float ySpacing = -3.3f;
        float xOffset = -6f; //initial x Offset
        float yOffset = 1f; //initial y Offset
        float cardScaling = 0.8f;

        UnrenderDecks();

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

        pa.TextUpdate("Select Your Cards:\nAvailable Points: " + points);
    }

    private void UnrenderDecks()
    {
        foreach (Transform child in cardArrayParent.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
