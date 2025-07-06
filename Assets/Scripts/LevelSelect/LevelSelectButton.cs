using LevelSelectInformation;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Represents a level select component that the player can click on to redirect them to a level 
public class LevelSelectButton : MonoBehaviour
{
    [field: SerializeField] public Level Level { get; set; }
    [SerializeField] private TMP_Text _textMeshPro;
    [SerializeField] private Outline outline;
    [SerializeField] private Vector2 defaultOutline;
    [SerializeField] private Vector2 hoverOutline;
    [SerializeField] private Color defaultOutlineColor = Color.white;
    [SerializeField] private Color hoverOutlineColor;
    [SerializeField] private Color defaultTextColor = Color.white;
    [SerializeField] private Color hoverTextColor;
    [SerializeField] private GameObject hover;
    [SerializeField] private GameObject lockedIndicator;
    [SerializeField] private GameObject notificationPrefab;
    [SerializeField] private string levelTitle;
    [SerializeField] private bool vanishWhenLocked = false;
#nullable enable
    private ILevelSelectInformation? LevelInformation { get => ILevelSelectInformation.LEVEL_INFORMATION.GetValueOrDefault(Level); }

    private const string LOCKED_TEXT = "LOCKED";
    private const string LOCKED_NOTIFICATION_TEXT = "Content Locked";

    private bool isLocked = false;
    
    public void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _textMeshPro.text = levelTitle;
        if (LevelInformation?.LevelID > GameStateManager.Instance.CurrentLevelProgress) Lock();
    }

    private void Lock()
    {
        _textMeshPro.text = LOCKED_TEXT;
        isLocked = true;
        // Show the lock indicator, but if it's a reference to this, then hide this button
        // Some buttons will have a locked indicator, while other buttons will disappear when locked
        // Used for hiding the bounty button when prerequiste levels aren't completed
        lockedIndicator.SetActive(false);
        if (vanishWhenLocked) gameObject.SetActive(false);
    }

    private void Unlock()
    {
        _textMeshPro.text = levelTitle;
        isLocked = false;
        lockedIndicator.SetActive(false);
        if (vanishWhenLocked) gameObject.SetActive(true);
    }

    public void SetHover(bool state) {
        if (isLocked) return;
        hover.SetActive(state);
        _textMeshPro.color = state ? hoverTextColor : defaultTextColor;
        if (outline != null) {
            outline.effectDistance = state ? hoverOutline : defaultOutline;
            outline.effectColor = state ? hoverOutlineColor : defaultOutlineColor;
        }
    }

    public void OnClick(BaseEventData BaseEventData)
    {
        PointerEventData pointerData = BaseEventData as PointerEventData;

        if (isLocked)
        {
            // Summon locked notifications
            FloatingNotification notification = Instantiate(notificationPrefab).GetComponent<FloatingNotification>();
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(pointerData.position);
            notification.Initialize(worldPos, LOCKED_NOTIFICATION_TEXT);
        } else
        {
            // Open scene otherwise
            LevelInformation?.UponSelectedEvent();
        }
    }
}