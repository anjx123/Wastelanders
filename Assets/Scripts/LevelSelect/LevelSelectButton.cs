using LevelSelectInformation;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Represents a level select component that the player can click on to redirect them to a level 
public class LevelSelectButton : MonoBehaviour, IPointerClickHandler
{
    [field: SerializeField] public Level Level { get; set; }
    [SerializeField] private TMP_Text _textMeshPro;
    [SerializeField] private string title;
    [SerializeField] private Button button;
    [SerializeField] private Outline outline;
    [SerializeField] private Vector2 defaultOutline;
    [SerializeField] private Vector2 hoverOutline;
    [SerializeField] private Color defaultOutlineColor = Color.white;
    [SerializeField] private Color hoverOutlineColor;
    [SerializeField] private Color defaultTextColor = Color.white;
    [SerializeField] private Color hoverTextColor;
    [SerializeField] private GameObject hover;
    [SerializeField] private GameObject lockedIndicator;
    [SerializeField] private float lockAnimationDuration = 0.5f;

    #nullable enable
    private ILevelSelectInformation? LevelInformation { get => ILevelSelectInformation.LEVEL_INFORMATION.GetValueOrDefault(Level); }

    public void Awake()
    {
        Initialize();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!button.enabled)
        {
            PopUpNotificationManager.Instance.DisplayWarning(PopupType.ContentLocked);
        }
    }

    public void OpenScene()
    {
        LevelInformation?.UponSelectedEvent();
    }

    private void Initialize()
    {
        if (LevelInformation?.LevelID > GameStateManager.Instance.CurrentLevelProgress) Lock();
    }

    public void Lock()
    {
        _textMeshPro.text = "LOCKED";
        button.enabled = false;
        // Show the lock indicator, but if it's a reference to this, then hide this button
        // Some buttons will have a locked indicator, while other buttons will disappear when locked
        // Used for hiding the bounty button when prerequiste levels aren't completed
        lockedIndicator.SetActive(!(lockedIndicator == gameObject));
    }

    public void Unlock(bool animate = false) {
        _textMeshPro.text = title;
        button.enabled = true;
        bool selfIsLock = lockedIndicator == gameObject;
        if (!animate) {
            lockedIndicator.SetActive(selfIsLock);
            return;
        }
        
        StartCoroutine(FadeLockIndicator(selfIsLock));
    }

    private IEnumerator FadeLockIndicator(bool show)
    {
        CanvasGroup canvasGroup = lockedIndicator.GetComponent<CanvasGroup>();

        float startAlpha = show ? 0f : 1f;
        float endAlpha = show ? 1f : 0f;

        if (show)
            lockedIndicator.SetActive(true);

        float elapsed = 0f;
        while (elapsed < lockAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / lockAnimationDuration);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }
        canvasGroup.alpha = endAlpha;

        if (!show)
            lockedIndicator.SetActive(false);
    }

    public void SetHover(bool state) {
        if (!button.enabled) return;
        hover.SetActive(state);
        outline.effectDistance = state ? hoverOutline : defaultOutline;
        outline.effectColor = state ? hoverOutlineColor : defaultOutlineColor;
        _textMeshPro.color = state ? hoverTextColor : defaultTextColor;
    }
}