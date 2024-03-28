using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using static CardDatabase;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using UnityEngine.AI;
using UnityEditor;

public class DeckSelectionManager : MonoBehaviour
{
    [SerializeField] private GameObject characterSelectionUi;
    [SerializeField] private GameObject weaponSelectionUi;
    [SerializeField] private GameObject deckSelectionUi;
    [SerializeField] private GameObject cardArrayParent;
    [SerializeField] private CardDatabase cardDatabase;
    [SerializeField] private PlayerDatabase playerDatabase;
    [SerializeField] private FadeScreenHandler fadeScreenHandler;
    private PlayerDatabase.PlayerData playerData;
    private WeaponType weaponType;
    public WeaponAmount weaponText;
    public PointsAmount pointsText;
    private bool isFadingOut = false;
    public static DeckSelectionManager Instance { get; private set; }
#nullable enable
    public delegate void PlayerActionDeckDelegate(int points);
    public event PlayerActionDeckDelegate? PlayerActionDeckModifiedEvent;

    private string nextScene = "LevelSelect";
    
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

    void Start()
    {
        ActionClass.CardClickedEvent += ActionSelected;
        CharacterSelect.CharacterSelectedEvent += CharacterChosen;
        WeaponSelect.WeaponSelectEvent += WeaponSelected;
        WeaponEdit.WeaponEditEvent += WeaponDeckEdit;
        DeckSelectionArrow.DeckSelectionArrowEvent += PrevState;

    }

    void OnDestroy()
    {
        ActionClass.CardClickedEvent -= ActionSelected;
        CharacterSelect.CharacterSelectedEvent -= CharacterChosen;
        WeaponSelect.WeaponSelectEvent -= WeaponSelected;
        WeaponEdit.WeaponEditEvent -= WeaponDeckEdit;
        DeckSelectionArrow.DeckSelectionArrowEvent -= PrevState;
    }

    private void PrevState()
    {
        if (DeckSelectionState == DeckSelectionState.WeaponSelection) {
            DeckSelectionState = DeckSelectionState.CharacterSelection;

        } else if (DeckSelectionState == DeckSelectionState.DeckSelection) {
            DeckSelectionState = DeckSelectionState.WeaponSelection;
        } else if (DeckSelectionState == DeckSelectionState.CharacterSelection) {
            EditorUtility.SetDirty(playerDatabase); //Probably will have to remove in production
            StartCoroutine(ExitDeckSelection());
        }
    }
    private IEnumerator ExitDeckSelection()
    {
        if (!isFadingOut)
        {
            isFadingOut = true;
            yield return StartCoroutine(fadeScreenHandler.FadeInDarkScreen(0.8f));
            SceneManager.LoadScene(nextScene);
            isFadingOut = false;
        }
    }
    public void SetNextScene(string newScene)
    {
        nextScene = newScene;
    }

    private void CharacterChosen(PlayerDatabase.PlayerName playerName)
    {
        playerData = playerDatabase.GetDataByPlayerName(playerName);
        DeckSelectionState = DeckSelectionState.WeaponSelection;
    }

    private void WeaponSelected(WeaponSelect c, CardDatabase.WeaponType weaponType)
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

    private void WeaponDeckEdit(CardDatabase.WeaponType weaponType)
    {
        this.weaponType = weaponType;
        RenderDecks(weaponType);
        DeckSelectionState = DeckSelectionState.DeckSelection;
    }

    private void ActionSelected(ActionClass ac)
    {
        List<SerializableWeaponListEntry> playerDeck = playerData.playerDeck;
        ActionClass pref = cardDatabase.GetCardsByType(weaponType).FirstOrDefault(action => action.GetType() == ac.GetType());
        SerializableWeaponListEntry entry = playerDeck.FirstOrDefault(entry => entry.key == weaponType);
        SerializableTuple<WeaponType, SerializableTuple<int, int>> proficiencyPointsTuple = playerData.playerWeaponProficiency.FirstOrDefault(entry => entry.Item1 == weaponType);
        
        if (entry == null)
        {
            entry = new SerializableWeaponListEntry()
            {
                key = weaponType,
                value = new List<ActionClass>()
            };
            playerDeck.Add(entry);
        }

        if (proficiencyPointsTuple == null) {
            proficiencyPointsTuple = new(weaponType, new(0, 0));
            playerData.playerWeaponProficiency.Add(proficiencyPointsTuple);
        }

        SerializableTuple<int, int> weaponPointTuple = proficiencyPointsTuple.Item2; //Left int represents currentPoints, right represents Max Points allowed
        ActionClass actionFound = entry.value.FirstOrDefault(action => action.GetType() == ac.GetType());
        if (actionFound != null) {
            weaponPointTuple.Item1 -= ac.CostToAddToDeck;          
            ac.SetSelectedForDeck(false);
            entry.value.Remove(actionFound);
        } else {
            if (weaponPointTuple.Item1 + ac.CostToAddToDeck <= weaponPointTuple.Item2) {
                weaponPointTuple.Item1 += ac.CostToAddToDeck;
                ac.SetSelectedForDeck(true);
                entry.value.Add(pref);
            } else {
                Debug.LogWarning("Insufficient experience points");
            }
        }

        int availablePoints = weaponPointTuple.Item2 - weaponPointTuple.Item1;
        pointsText.TextUpdate("Select Your Cards:\nAvailable Points: " + availablePoints.ToString());

        PlayerActionDeckModifiedEvent?.Invoke(availablePoints);
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
            ActionClass ac = go.GetComponent<ActionClass>();
            ActionClass pref = chosenCardList.FirstOrDefault(action => action.GetType() == card.GetType());
            if (pref != null) {
                ac.SetSelectedForDeck(true);
            }
            ac.SetRenderCost(true);
            ac.UpdateDup();
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
        SerializableTuple<WeaponType, SerializableTuple<int, int>> tuple = playerData.playerWeaponProficiency.FirstOrDefault(entry => entry.Item1 == weaponType);
        int availablePoints = 0;
        if (tuple != null) {
            availablePoints = tuple.Item2.Item2 - tuple.Item2.Item1;
        }

        pointsText.TextUpdate("Select Your Cards:\nAvailable Points: " + availablePoints);
    }

    private void UnrenderDecks()
    {
        foreach (Transform child in cardArrayParent.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
