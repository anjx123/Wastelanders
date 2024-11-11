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
        _textMeshPro.text = "Locked";
        button.enabled = false;
    }
}