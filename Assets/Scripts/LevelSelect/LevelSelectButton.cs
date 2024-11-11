using LevelSelectInformation;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class LevelSelectButton : MonoBehaviour
{
    [field: SerializeField] public Level Level { get; set; }
    [SerializeField] private TMP_Text _textMeshPro;
    [SerializeField] private Button button;
    private ILevelSelectInformation LevelInformation { get => ILevelSelectInformation.LEVEL_INFORMATION.GetValueOrDefault(Level); }

    public void Start()
    {
        Initialize();
    }
    public void OpenScene()
    {
        LevelInformation.OpenScene();
    }

    private void Initialize()
    {
        if (LevelInformation.LevelID > GameStateManager.Instance.CurrentLevelProgress) Lock();
    }

    private void Lock()
    {
        _textMeshPro.text = "Locked";
        button.enabled = false;
    }
}