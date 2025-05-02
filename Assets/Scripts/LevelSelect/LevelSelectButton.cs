using LevelSelectInformation;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Represents a level select component that the player can click on to redirect them to a level 
public class LevelSelectButton : MonoBehaviour
{
    [field: SerializeField] public Level Level { get; set; }
    [SerializeField] private TMP_Text _textMeshPro;
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
#nullable enable
    private ILevelSelectInformation? LevelInformation { get => ILevelSelectInformation.LEVEL_INFORMATION.GetValueOrDefault(Level); }

    public void Start()
    {
        Initialize();
    }
    public void OpenScene()
    {
        LevelInformation?.UponSelectedEvent();
    }

    private void Initialize()
    {
        if (LevelInformation?.LevelID > GameStateManager.Instance.CurrentLevelProgress) Lock();
    }

    private void Lock()
    {
        _textMeshPro.text = "LOCKED";
        button.enabled = false;
        // Show the lock indicator, but if it's a reference to this, then hide this button
        // Some buttons will have a locked indicator, while other buttons will disappear when locked
        // Used for hiding the bounty button when prerequiste levels aren't completed
        lockedIndicator.SetActive(!(lockedIndicator == gameObject));
    }

    public void SetHover(bool state) {
        if (!button.enabled) return;
        hover.SetActive(state);
        outline.effectDistance = state ? hoverOutline : defaultOutline;
        outline.effectColor = state ? hoverOutlineColor : defaultOutlineColor;
        _textMeshPro.color = state ? hoverTextColor : defaultTextColor;
    }
}