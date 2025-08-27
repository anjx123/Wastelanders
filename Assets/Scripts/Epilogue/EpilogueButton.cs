using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class EpilogueButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image lockIndicator;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text requirementText;
    private int completedBounties;
    private int neededBounties;
    private EpilogueSceneData sceneData;


    public void Bind(EpilogueSceneData epilogueSceneData)
    {
        sceneData = epilogueSceneData;
        neededBounties = sceneData.BountyRequirement;
        completedBounties = PrincessFrogBounties.Values.Count(b => BountyManager.Instance.IsBountyCompleted(b));
        button.onClick.AddListener(() =>
        {
            BountyManager.Instance.GoToEpilogueScene(epilogueSceneData);
        });
        SetTitle(sceneData.EpilogueTitle);
        SetLocked(neededBounties > completedBounties);
        UpdateRequirementText();
    }

    private void SetTitle(string text)
    {
        titleText.SetText($"{text}");
    }

    private void SetLocked(bool state)
    {
        lockIndicator.enabled = state;
        button.interactable = !state;
    }

    private void UpdateRequirementText()
    {
        requirementText.SetText($"{completedBounties}/{neededBounties} bounties");
    }
}
