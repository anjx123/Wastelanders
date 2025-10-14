
using UnityEngine;


public class PopUpNotificationManager : MonoBehaviour
{
    public static PopUpNotificationManager Instance { get; private set; }

    public GameObject floatingNotificationPrefab;

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

    public void DisplayWarning(PopupType popupType, GameObject obj = null)
    {
        string message = popupType switch
        {
            PopupType.SameSpeed => "Same Speed Selected!",
            PopupType.EnemyKilled => "Enemy Killed!",
            PopupType.DeckReshuffled => "Deck Reshuffled!",
            PopupType.SelectActionFirst => "Select an Action first!",
            PopupType.SelectPlayerFirst => "Select a Player first!",
            PopupType.InsufficientResources => "Insufficient resources!",
            _ => string.Empty
        };

        if (!string.IsNullOrEmpty(message))
        {
            CreateFloatingWarning(message);
        }
    }

    private void CreateFloatingWarning(string message) {
        Vector3 spawnPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameObject notification = Instantiate(floatingNotificationPrefab, spawnPosition, Quaternion.identity);
        FloatingNotification floatingNotification = notification.GetComponent<FloatingNotification>();
        floatingNotification.Initialize(spawnPosition, message);
    }
}