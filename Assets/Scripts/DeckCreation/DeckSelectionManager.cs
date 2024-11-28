using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardDatabase;
using UnityEngine.SceneManagement;
using System.Linq;
using Systems.Persistence;
using WeaponDeckSerialization;
using UnityEditor;
using System;
using TMPro;

public class DeckSelectionManager : MonoBehaviour
{
    [SerializeField] private GameObject characterSelectionUi;
    [SerializeField] private GameObject weaponSelectionUi;
    [SerializeField] private GameObject deckSelectionUi;
    [SerializeField] private GameObject cardArrayParent;
    [SerializeField] private CardDatabase cardDatabase;
    [SerializeField] private PlayerDatabase playerDatabase;
    [SerializeField] private FadeScreenHandler fadeScreenHandler;
    [SerializeField] private TMP_Text cardDescriptorTextField;
    [SerializeField] private Transform[] enemyCardLayout;
    [SerializeField] private Transform[] fourRowCardLayout;
    [SerializeField] private Transform[] fiveRowCardLayout;
    [SerializeField] private Transform enemyEditParent;
    [SerializeField] private GameObject enemyEditButtonPrefab;
    private PlayerDatabase.PlayerData playerData;
    private WeaponType weaponType;
    public WeaponAmount weaponText;
    public PointsAmount pointsText;
    public BuffExplainer buffExplainer;
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
        ActionClass.CardRightClickedEvent += CardRightClicked;
        ActionClass.CardHighlightedEvent += RenderCardDescription;
        ActionClass.CardUnhighlightedEvent += RemoveCardDescription;
        CharacterSelect.CharacterSelectedEvent += CharacterChosen;
        WeaponSelect.WeaponSelectEvent += WeaponSelected;
        WeaponEdit.WeaponEditEvent += WeaponDeckEdit;
        DeckSelectionArrow.DeckSelectionArrowEvent += PrevState;
    }

    void OnDestroy()
    {
        ActionClass.CardClickedEvent -= ActionSelected;
        ActionClass.CardRightClickedEvent -= CardRightClicked;
        ActionClass.CardHighlightedEvent -= RenderCardDescription;
        ActionClass.CardUnhighlightedEvent -= RemoveCardDescription;
        CharacterSelect.CharacterSelectedEvent -= CharacterChosen;
        WeaponSelect.WeaponSelectEvent -= WeaponSelected;
        WeaponEdit.WeaponEditEvent -= WeaponDeckEdit;
        DeckSelectionArrow.DeckSelectionArrowEvent -= PrevState;
    }

    private void PrevState()
    {
        if (DeckSelectionState == DeckSelectionState.WeaponSelection)
        {
            DeckSelectionState = DeckSelectionState.CharacterSelection;
        }
        else if (DeckSelectionState == DeckSelectionState.DeckSelection)
        {
            DeckSelectionState = DeckSelectionState.WeaponSelection;
        }
        else if (DeckSelectionState == DeckSelectionState.CharacterSelection)
        {
            // Save user Data here
            StartCoroutine(ExitDeckSelection());
        }
    }
    private IEnumerator ExitDeckSelection()
    {
        if (!isFadingOut)
        {
            isFadingOut = true;
            SaveLoadSystem.Instance.SaveGame();
            EditorUtility.SetDirty(playerDatabase); // For easily resetting the default weaponDeck of playerDatabase
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
        if (playerData.selectedWeapons.Contains(weaponType))
        {
            playerData.selectedWeapons.Remove(weaponType);
            c.SetSelected(false);
            weaponText.TextUpdate(playerData.selectedWeapons.Count.ToString() + "/2 Selected");
        }
        else if (playerData.selectedWeapons.Count < 2)
        {
            playerData.selectedWeapons.Add(weaponType);
            c.SetSelected(true);
            weaponText.TextUpdate(playerData.selectedWeapons.Count.ToString() + "/2 Selected");
        }
        else
        {
            Debug.LogWarning("Can only select 2 weapons");
        }
    }

    private void WeaponDeckEdit(CardDatabase.WeaponType weaponType)
    {
        buffExplainer.RenderExplanationForBuff(weaponType);
        this.weaponType = weaponType;
        DeckSelectionState = DeckSelectionState.DeckSelection;
        RenderDecks(weaponType);
    }

    private SerializableWeaponListEntry GetPlayerWeaponDeck(WeaponType weaponType)
    {
        SerializableWeaponListEntry playerWeaponDeck = playerData.playerDeck.FirstOrDefault(entry => entry.weapon == weaponType);

        if (playerWeaponDeck == null)
        {
            playerWeaponDeck = new SerializableWeaponListEntry()
            {
                weapon = weaponType,
                weaponDeck = new List<SerializableActionClassInfo>()
            };
            playerData.playerDeck.Add(playerWeaponDeck);
        }

        return playerWeaponDeck;
    }

    private WeaponProficiency GetProficiencyPointsTuple(WeaponType weaponType)
    {
        var proficiencyPointsTuple = playerData.playerWeaponProficiency.FirstOrDefault(entry => entry.WeaponType == weaponType);
        if (proficiencyPointsTuple == null)
        {
            proficiencyPointsTuple = new WeaponProficiency(weaponType, 0, 0);
            playerData.playerWeaponProficiency.Add(proficiencyPointsTuple);
        }
        return proficiencyPointsTuple;
    }

    private bool DeckContainsCard(ActionClass ac)
    {
        return GetPlayerWeaponDeck(weaponType).weaponDeck.FirstOrDefault(action => action.ActionClassName == ac.GetType().Name) != null;
    }

    private void OnUpdateDeck(WeaponProficiency weaponPointTuple)
    {
        int availablePoints = weaponPointTuple.MaxPoints - weaponPointTuple.CurrentPoints;
        pointsText.TextUpdate("Select Your Cards\nAvailable Points: <color=#FFD700>" + availablePoints.ToString() + "</color>");

        PlayerActionDeckModifiedEvent?.Invoke(availablePoints);
    }

    // PERF: DeckContainsCard finds an actionFound but in doesn't return it, which is searched for again here.
    private void DeselectFromDeck(ActionClass ac, bool performChecks = true)
    {
        SerializableWeaponListEntry playerWeaponDeck = GetPlayerWeaponDeck(weaponType);
        WeaponProficiency weaponPointTuple = GetProficiencyPointsTuple(weaponType);

        if (!performChecks || DeckContainsCard(ac))
        {
            weaponPointTuple.CurrentPoints -= ac.CostToAddToDeck;
            ac.SetSelectedForDeck(false);
            var actionFound = playerWeaponDeck.weaponDeck.FirstOrDefault(action => action.ActionClassName == ac.GetType().Name);
            playerWeaponDeck.weaponDeck.Remove(actionFound);
            OnUpdateDeck(weaponPointTuple);
        }
    }

    private void AddToDeck(ActionClass ac, bool performChecks = true)
    {
        SerializableWeaponListEntry playerWeaponDeck = GetPlayerWeaponDeck(weaponType);
        WeaponProficiency weaponPointTuple = GetProficiencyPointsTuple(weaponType);

        // Do we have sufficient points? If so, are we trying to add the evolved form? If so, is the evolution progress sufficient?
        if ((!performChecks || weaponPointTuple.CurrentPoints + ac.CostToAddToDeck <= weaponPointTuple.MaxPoints) && (!ac.IsFlipped || (ac.IsFlipped && ac.CanEvolve())))
        {
            weaponPointTuple.CurrentPoints += ac.CostToAddToDeck;
            ac.SetSelectedForDeck(true);
            playerWeaponDeck.weaponDeck.Add(new(ac.GetType().Name, ac.IsFlipped && ac.CanEvolve()));
            OnUpdateDeck(weaponPointTuple);
        }
        else
        {
            Debug.LogWarning("Insufficient experience points");
        }
    }

    private void FlipCard(ActionClass ac)
    {
        ac.IsFlipped = !ac.IsFlipped; // Invert IsFlipped status
        ac.cardUI.RenderCard(ac); // Re-render the card based on the new flipped state
    }

    private void CardRightClicked(ActionClass ac)
    {
        // Deselect the card as to not allow duplicate selecting
        DeselectFromDeck(ac);
        // Flip the card itself
        FlipCard(ac);
        // Re-call OnMouseEnter so that we re-render the card description popup
        ac.OnMouseEnter();
    }

    // Handles when a card is clicked event
    private void ActionSelected(ActionClass ac)
    {
        if (DeckContainsCard(ac))
        {
            DeselectFromDeck(ac, false);
        }
        else
        {
            AddToDeck(ac);
        }
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

    private void RenderCardDescription(ActionClass card)
    {
        cardDescriptorTextField.text = card.GenerateCardDescription();
    }

    private void RemoveCardDescription(ActionClass card)
    {
        cardDescriptorTextField.text = "";
    }

    private WeaponType[] GetEnemyWeaponTypes()
    {
        return new WeaponType[] { WeaponType.FROG, WeaponType.BEETLE, WeaponType.SLIME };
    }

    private bool IsEnemyCardType(WeaponType weaponType)
    {
        return GetEnemyWeaponTypes().Contains(weaponType);
    }

    private void GenerateEnemyButtons()
    {
        WeaponType[] weaponTypes = GetEnemyWeaponTypes();
        List<BuffExplainer.WeaponExplanation> explanations = buffExplainer.explanationText.FindAll((e) => weaponTypes.Contains(e.WeaponType));

        float y = 0f;
        float delta = -1f;
        foreach (BuffExplainer.WeaponExplanation explanation in explanations)
        {
            GameObject button = Instantiate(enemyEditButtonPrefab);
            button.transform.SetParent(enemyEditParent);
            button.transform.localPosition = new Vector3(0, y, 0);
            y += delta;

            WeaponEdit weaponEdit = button.GetComponentInChildren<WeaponEdit>();
            weaponEdit.editText.SetText(explanation.ExplanationTitle);
            weaponEdit.SetType(explanation.WeaponType);
        }
    }

    //Renders the weaponDeck corresponding to (@param weaponType)
    public void RenderDecks(CardDatabase.WeaponType weaponType)
    {
        UnrenderDecks();

        List<ActionClass> chosenCardList = cardDatabase.ConvertStringsToCards(weaponType, playerData.GetDeckByWeaponType(weaponType).Select(p => p.ActionClassName).ToList());
        List<ActionClass> cardsToRender = cardDatabase.GetCardsByType(weaponType);
        List<GameObject> instantiatedCards = new List<GameObject>();
        Transform[] layout;

        if (IsEnemyCardType(weaponType))
        {
            layout = enemyCardLayout;
            GenerateEnemyButtons();
        }
        else
        {
            layout = cardsToRender.Count > 8 ? fiveRowCardLayout : fourRowCardLayout;
        }

        //In order to sort, the cards must be instantiated and initialized first :pensive:
        foreach (ActionClass card in cardsToRender)
        {
            GameObject go = Instantiate(card.gameObject);
            instantiatedCards.Add(go);
            ActionClass ac = go.GetComponent<ActionClass>();
            ActionClass pref = chosenCardList.FirstOrDefault(action => action.GetType() == card.GetType());
            if (pref != null)
            {
                ac.SetSelectedForDeck(true);
            }
            ac.SetRenderCost(true);
            ac.UpdateDup();
        }

        instantiatedCards.Sort((card1, card2) => card1.GetComponent<ActionClass>().Speed.CompareTo(card2.GetComponent<ActionClass>().Speed));

        for (int i = 0; i < instantiatedCards.Count; i++)
        {
            GameObject cardPrefab = instantiatedCards[i];
            cardPrefab.transform.SetParent(cardArrayParent.transform);
            cardPrefab.transform.localPosition = layout[i].localPosition;
            cardPrefab.transform.localScale = layout[i].localScale;
        }

        SaveLoadSystem.Instance.LoadCardEvolutionProgress();

        WeaponProficiency weaponPointTuple = GetProficiencyPointsTuple(weaponType);
        // int availablePoints = weaponPointTuple.MaxPoints - weaponPointTuple.CurrentPoints;
        // pointsText.TextUpdate("Select Your Cards\nAvailable Points: <color=#FFD700>" + availablePoints + "</color>");

        OnUpdateDeck(weaponPointTuple);
    }

    private void UnrenderDecks()
    {
        foreach (Transform child in cardArrayParent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in enemyEditParent)
        {
            Destroy(child.gameObject);
        }
    }
}
