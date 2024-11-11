using BountySystem;
using LevelSelectInformation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    public LevelSelectButton[] buttons;
    public GameObject levelButtons;
    [SerializeField] private FadeScreenHandler fadeScreen;

    protected void Awake()
    {
        ButtonArray();
        fadeScreen.SetDarkScreen();
        StartCoroutine(fadeScreen.FadeInLightScreen(1f));
    }

    public void OnEnable()
    {
        StageInformation.StageInformationEvent += OpenScene;
        BountyInformation.BountyInformationEvent += OpenBountyByTypeName;
    }
    public void OnDisable()
    {
        StageInformation.StageInformationEvent -= OpenScene;
        BountyInformation.BountyInformationEvent -= OpenBountyByTypeName;
    }

    protected void OpenScene(string s)
    {
        StartCoroutine(FadeLevelIn(s));
    }

    private void OpenBountyByTypeName(Type type)
    {
        BountyManager.Instance.SelectedBountyType = type;
        StartCoroutine(FadeLevelIn(GameStateManager.CONTRACT_SELECT_NAME));
    }

    public void DeckSelect()
    {
        OpenScene(GameStateManager.SELECTION_SCREEN_NAME);
    }

    IEnumerator FadeLevelIn(string levelName)
    {
        yield return StartCoroutine(fadeScreen.FadeInDarkScreen(0.8f));
        GameStateManager.Instance.LoadScene(levelName);
    }

    void ButtonArray()
    {
        int children = levelButtons.transform.childCount;
        buttons = new LevelSelectButton[children];
        for (int i = 0; i < children; i++)
        {
            buttons[i] = levelButtons.transform.GetChild(i).gameObject.GetComponent<LevelSelectButton>();
        }
    }
}