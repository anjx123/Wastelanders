
using UnityEngine;


public class PopUpNotificationManager : MonoBehaviour
{
    public static PopUpNotificationManager Instance { get; private set; }

    public GameObject warningObject;
    public GameObject evolveObject;

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
        GetComponent<Canvas>().worldCamera = Camera.main;
        GetComponent<Canvas>().sortingLayerName = "CardUILayer";
    }

    private void Start()
    {
        CombatManager.OnGameStateChanged += DismissDescription;
        ActionClass.CardEvolvedNotifEvent += DisplayWarning;
        RemoveDescription();
    }

    private void OnDestroy()
    {
        CombatManager.OnGameStateChanged -= DismissDescription;
        ActionClass.CardEvolvedNotifEvent -= DisplayWarning;
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
            case PopupType.CardEvolved:
                if (obj == null) break;
                CreateNotification(obj.GetComponent<ActionClass>().GetIcon());
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

    public void CreateNotification(Sprite spr)
    {
        if (spr == null) return;
        StartCoroutine(evolveObject.GetComponent<EvolveInfo>().ShowEvolve(spr, "Card Has Evolved!"));
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