using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal;
using UnityEngine;


public class PopUpNotificationManager : MonoBehaviour
{
    public static PopUpNotificationManager Instance { get; private set; }

    public GameObject warningObject;
    public GameObject canvasObject;

    public bool isRunning = false;

    // Awake is called when the script instance is being loaded
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

        DontDestroyOnLoad(gameObject);
        createWarning("Round Start!!!");
    }

    private void Start()
    {
        CombatManager.Instance.OnGameStateChanged += DismissDescription;
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnGameStateChanged -= DismissDescription;
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
            case PopupType.SelectEnemyFirst: 
                createWarning("Select a card first!");
                break;
            default:
                break;
        }

    }

    public void createWarning(string message)
    {
        WarningInfo info = warningObject.GetComponent<WarningInfo>();
        info.showWarning(message);
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