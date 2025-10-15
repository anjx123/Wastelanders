
using UnityEngine;


public class PopUpNotificationManager : MonoBehaviour
{
    public static PopUpNotificationManager Instance { get; private set; }

    [SerializeField] private PopUpNotification floatingNotificationPrefab;
    [SerializeField] private Canvas canvas;

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

    public void DisplayWarning(PopupType popupType)
    {
        string message = popupType switch
        {
            PopupType.SameSpeed => "Same Speed Selected!",
            PopupType.EnemyKilled => "Enemy Killed!",
            PopupType.DeckReshuffled => "Deck Reshuffled!",
            PopupType.SelectActionFirst => "Select an Action first!",
            PopupType.SelectPlayerFirst => "Select a Player first!",
            PopupType.InsufficientResources => "Insufficient resources!",
            PopupType.ContentLocked => "Content Locked!",
            _ => string.Empty
        };

        if (!string.IsNullOrEmpty(message))
        {
            CreateFloatingWarning(message);
        }
    }

    private void CreateFloatingWarning(string message)
    {
        PopUpNotification notification = Instantiate(floatingNotificationPrefab, canvas.transform);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 localPoint
        );

        notification.Initialize(localPoint, message);
    }

}