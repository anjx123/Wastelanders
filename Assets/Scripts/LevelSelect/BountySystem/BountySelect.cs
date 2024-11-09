using BountySystem;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Class that handles the rendering and scene management of contract selection
public class BountySelect : MonoBehaviour
{
    [SerializeField] private FadeScreenHandler fadeScreen;
    [SerializeField] private RectTransform bountyGrid;
    [SerializeField] private BountyButton bountyButtonPrefab;

    protected virtual void Awake()
    {
        ConstructBountyButtons();
        fadeScreen.SetDarkScreen();
        StartCoroutine(fadeScreen.FadeInLightScreen(1f));
    }

    public void ConstructBountyButtons()
    {
        IBounties.MapOnValues(bounty => CreateButtonForBounty(bounty));
    }

    private void CreateButtonForBounty(IBounties bounty)
    {
        if (bounty.GetType().Name == BountyManager.Instance.SelectedBountyTypeName)
        {
            BountyButton bountyButton = Instantiate(bountyButtonPrefab);
            bountyButton.Initialize(bounty);
            bountyButton.transform.SetParent(bountyGrid, false);
        }
    }

    public void OpenScene(string s)
    {
        StartCoroutine(FadeLevelIn(s));
    }

    public void StartLevel()
    {
        OpenScene(BountyManager.Instance.ActiveBounty.SceneName);
    }

    IEnumerator FadeLevelIn(string levelName)
    {
        yield return StartCoroutine(fadeScreen.FadeInDarkScreen(0.8f));
        SceneManager.LoadScene(levelName);
    }

}
