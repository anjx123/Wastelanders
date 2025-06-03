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
    [SerializeField] private FadeScreenHandler fadeScreen;

    protected void Awake()
    {
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

    private void OpenBountyByTypeName(BountyInformation bountyInformation)
    {
        BountyManager.Instance.SelectedBountyInformation = bountyInformation;
        StartCoroutine(FadeLevelIn(GameStateManager.CONTRACT_SELECT_NAME));
    }

    public void DeckSelect()
    {
        OpenScene(GameStateManager.SELECTION_SCREEN_NAME);
    }

    public void MainMenu()
    {
        OpenScene(GameStateManager.MAIN_MENU_NAME);
    }

    public void Tutorial()
    {
        OpenScene(GameStateManager.TUTORIAL_FIGHT);
    }

    public void LevelSelectScene()
    {
        OpenScene(GameStateManager.LEVEL_SELECT_NAME);
    }

    public void PrincessFrogBounties()
    {
        OpenBountyByTypeName(BountyInformation.PRINCESS_FROG_BOUNTY);
    }

    IEnumerator FadeLevelIn(string levelName)
    {
        yield return StartCoroutine(fadeScreen.FadeInDarkScreen(0.6f));
        GameStateManager.Instance.LoadScene(levelName);
    }
}