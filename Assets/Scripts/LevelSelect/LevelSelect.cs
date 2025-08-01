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
        StartCoroutine(FadeLevelIn(SceneData.Get<SceneData.ContractSelect>().SceneName));
    }

    public void DeckSelect()
    {
        OpenScene(SceneData.Get<SceneData.SelectionScreen>().SceneName);
    }

    public void MainMenu()
    {
        OpenScene(SceneData.Get<SceneData.MainMenu>().SceneName);
    }

    public void Tutorial()
    {
        OpenScene(SceneData.Get<SceneData.TutorialFight>().SceneName);
    }

    public void LevelSelectScene()
    {
        OpenScene(SceneData.Get<SceneData.LevelSelect>().SceneName);
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