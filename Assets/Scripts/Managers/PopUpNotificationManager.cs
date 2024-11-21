
using UnityEngine;


public class PopUpNotificationManager : MonoBehaviour
{
    public static PopUpNotificationManager Instance { get; private set; }

    public GameObject warningObject;

    public bool isRunning = false;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CombatManager.OnGameStateChanged += DismissDescription;
        ActionClass.CardHighlightedEvent += OnCardHighlight;
        ActionClass.CardUnhighlightedEvent += OnCardUnhighlight;
        RemoveDescription();
    }

    private void OnDestroy()
    {
        CombatManager.OnGameStateChanged -= DismissDescription;
        ActionClass.CardHighlightedEvent -= OnCardHighlight;
        ActionClass.CardUnhighlightedEvent -= OnCardUnhighlight;
    }

    private void OnCardHighlight(ActionClass card)
    {
        RemoveDescription();
        DisplayText(card.GenerateCardDescription());
    }

    private void OnCardUnhighlight(ActionClass card)
    {
        RemoveDescription();
    }

    public void DisplayWarning(PopupType popupType, GameObject obj = null)
    {
        switch (popupType)
        {
            case PopupType.SameSpeed:
                createWarning("Multiple Cards with Same Speed Selected");
                break;
            case PopupType.EnemyKilled:
                createWarning("Enemy Killed!");
                break;
            case PopupType.DeckReshuffled:
                createWarning("Deck Reshuffled");
                break;
            case PopupType.SelectActionFirst:
                createWarning("Select a card first!");
                break;
            case PopupType.SelectPlayerFirst:
                createWarning("Select a player first!");
                break;
            default:
                break;
        }

    }

    public void createWarning(string message)
    {
        WarningInfo info = warningObject.GetComponent<WarningInfo>();
        info.ShowWarning(message);
    }

    public void DisplayText(string description)
    {
        warningObject.GetComponent<WarningInfo>().DisplayDescription(description);
    }

    public void RemoveDescription()
    {
        warningObject.GetComponent<WarningInfo>().RemoveDescription();
    }

    public void DismissDescription(GameState gameState)
    {
        if (gameState == GameState.FIGHTING)
        {
            RemoveDescription();
        }
    }

}