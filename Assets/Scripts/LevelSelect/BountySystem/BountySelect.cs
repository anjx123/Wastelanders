using BountySystem;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Class that handles the rendering and scene management of bounty selection
public class BountySelect : MonoBehaviour
{
    [SerializeField] private FadeScreenHandler fadeScreen;
    [SerializeField] private Transform[] bountyGrid;
    [SerializeField] private BountyButton bountyButtonPrefab;
    [SerializeField] private BountyRewardIconDatabase bountyRewardIconDatabase;


    [SerializeField] private TMP_Text bountyTitle;
    [SerializeField] private TMP_Text bountyDescription;
    [SerializeField] private TMP_Text bountyRewards;
    [SerializeField] private TMP_Text chooseBountyText;

#nullable enable
    protected virtual void Awake()
    {
        BountyButton.BountyOnHoverEvent += OnBountyHover;
        BountyButton.BountyOnHoverEndEvent += OnBountyHoverEnd;
        ConstructBountyButtons();
        fadeScreen.SetDarkScreen();
        StartCoroutine(fadeScreen.FadeInLightScreen(1f));
    }

    void OnDestroy()
    {
        BountyButton.BountyOnHoverEvent -= OnBountyHover;
        BountyButton.BountyOnHoverEndEvent -= OnBountyHoverEnd;
    }

    public void ConstructBountyButtons()
    {
        var bountyCollection = IBounties.Values.SelectMany(innerList => innerList).ToList();
        if (bountyCollection == null) return;

        int ind = 0;
        foreach (var bounty in bountyCollection)
        {
            BountyButton b = CreateButtonForBounty(bounty);
            b.transform.SetParent(bountyGrid[ind++]);
            b.transform.localPosition = Vector3.zero;
            Sprite s = bountyRewardIconDatabase.pairs.Find((match) => match.BountyName == bounty.BountyName).Icon;
            b.SetRewardIcon(s);
        }
    }

    private BountyButton CreateButtonForBounty(IBounties bounty)
    {
        BountyButton bountyButton = Instantiate(bountyButtonPrefab);
        bountyButton.Initialize(bounty);
        return bountyButton;
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
        }
        else
        {
            // TODO: Say select a bounty first !
        }
    }

    IEnumerator FadeLevelIn(string levelName)
    {
        yield return StartCoroutine(fadeScreen.FadeInDarkScreen(0.8f));
        SceneManager.LoadScene(levelName);
    }

    private void ClearPopupText()
    {
        bountyTitle.gameObject.SetActive(false);
        bountyDescription.gameObject.SetActive(false);
        bountyRewards.gameObject.SetActive(false);

        chooseBountyText.gameObject.SetActive(true);
    }

    private void SetPopupText(IBounties? bounty)
    {
        if (bounty == null)
        {
            ClearPopupText();
            return;
        }

        bountyTitle.SetText(bounty.BountyName);
        bountyTitle.gameObject.SetActive(true);

        bountyDescription.SetText(bounty.FlavourText);
        bountyDescription.gameObject.SetActive(true);

        bountyRewards.SetText($"<color=#FF0>Rewards:</color>\n{bounty.Rewards}");
        bountyRewards.gameObject.SetActive(true);

        chooseBountyText.gameObject.SetActive(false);
    }


    // We need to update the popup text
    private void OnBountyHover(IBounties bounty)
    {
        SetPopupText(bounty);
    }

    // We need to remove the popup text, and replace it with the selected bounty text if 
    // a bounty is selected
    private void OnBountyHoverEnd(IBounties bounty)
    {
        SetPopupText(BountyManager.Instance.ActiveBounty);
    }

}
