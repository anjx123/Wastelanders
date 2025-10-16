
using UnityEngine;


public class PopUpNotificationManager : MonoBehaviour
{
    public static PopUpNotificationManager Instance { get; private set; }

    public WarningInfo warningInfo;

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
        RemoveDescription();
    }

    private void OnDestroy()
    {
        CombatManager.OnGameStateChanged -= DismissDescription;
    }

    public void DisplayWarning(PopupType popupType, GameObject obj = null)
    {
        string message = popupType switch
        {
            PopupType.SameSpeed => "Multiple Cards with Same Speed Selected",
            PopupType.EnemyKilled => "Enemy Killed!",
            PopupType.DeckReshuffled => "Deck Reshuffled",
            PopupType.SelectActionFirst => "Select a card first!",
            PopupType.SelectPlayerFirst => "Select a player first!",
            PopupType.InsufficientResources => "Insufficient resources!",
            _ => string.Empty
        };

        if (!string.IsNullOrEmpty(message))
        {
            CreateWarning(message);
        }
    }

    private void CreateWarning(string message)
    {
        warningInfo.ShowWarning(message);
    }

    public void RemoveDescription()
    {
        warningInfo.RemoveDescription();
    }

    public void DismissDescription(GameState gameState)
    {
        if (gameState == GameState.FIGHTING)
        {
            RemoveDescription();
        }
    }

}