using BountySystem;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Class that handles the rendering and scene management of bounty selection
public class BountySelect : MonoBehaviour
{
    [SerializeField] private FadeScreenHandler fadeScreen;
    [SerializeField] private RectTransform bountyGrid;
    [SerializeField] private BountyButton bountyButtonPrefab;

#nullable enable
    protected virtual void Awake()
    {
        ConstructBountyButtons();
        fadeScreen.SetDarkScreen();
        StartCoroutine(fadeScreen.FadeInLightScreen(1f));
    }

    public void ConstructBountyButtons()
    {
        var bountyCollection = BountyManager.Instance.SelectedBountyInformation?.BountyCollection;
        if (bountyCollection == null) return;

        foreach (var bounty in bountyCollection)
        {
            CreateButtonForBounty(bounty);
        }
    }

    private void CreateButtonForBounty(IBounties bounty)
    {
        BountyButton bountyButton = Instantiate(bountyButtonPrefab);
        bountyButton.Initialize(bounty);
        bountyButton.transform.SetParent(bountyGrid, false);
    }

    public void OpenScene(string s)
    {
        StartCoroutine(FadeLevelIn(s));
    }

    public void StartLevel()
    {
        if (BountyManager.Instance.ActiveBounty != null)
        {
            OpenScene(BountyManager.Instance.ActiveBounty.SceneName);
        } else
        {
            // TODO: Say select a bounty first !
        }
    }

    IEnumerator FadeLevelIn(string levelName)
    {
        yield return StartCoroutine(fadeScreen.FadeInDarkScreen(0.8f));
        SceneManager.LoadScene(levelName);
    }

}
