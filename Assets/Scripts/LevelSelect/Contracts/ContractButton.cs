using System;
using UnityEngine;
using UnityEngine.UI;
using static Contracts;
using static Challenges;

// Originally was used to toggle specific contracts, repurposed to toggle specific challenges
public class ContractButton : MonoBehaviour
{
    // The level 0-based index, 0 being tutorial, 3 being queen
    // Temp solution to distinguish and enable button layouts
    public int level;
    public string contract;
    private bool selected;

    private ColorBlock activeColors;
    private ColorBlock inactiveColors;
    private const float INACTIVE_ALPHA = 0.5f;

    protected virtual void Awake()
    {
        // hmmmm uhh hmmm, prolly a better way to do this through unity and not just script
        activeColors = GetComponent<Button>().colors;
        inactiveColors = activeColors;
        var inactiveNormalColor = inactiveColors.normalColor;
        inactiveNormalColor.a = INACTIVE_ALPHA;
        inactiveColors.normalColor = inactiveNormalColor;
        inactiveColors.selectedColor = inactiveNormalColor;

        GetComponent<Button>().colors = selected ? activeColors : inactiveColors;
    }

    public void OnSelect() 
    {
        selected = !selected;
        switch (level)
        {
            case 0:
                Debug.LogError("No Tutorial Contracts!");
                break;
            case 1:
                ToggleChallenge(StringToChallenge<FrogChallenges>(contract));
                break;
            case 2:
                ToggleChallenge(StringToChallenge<BeetleChallenges>(contract));
                break;
            case 3:
                ToggleChallenge(StringToChallenge<QueenChallenges>(contract));
                break;
        }

        GetComponent<Button>().colors = selected ? activeColors : inactiveColors;
    }

    T StringToChallenge<T>(string challenge) where T : Enum
    {
        return (T) Enum.Parse(typeof(T), challenge);
    }

    void ToggleChallenge(Enum challenge) 
    {
        if (selected) {
            string flavourText = ContractManager.Instance.SetActiveChallenge(challenge);
            Debug.Log(flavourText);
        } else {
            ContractManager.Instance.SetActiveChallenge(null);
        }
    }

}