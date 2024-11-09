using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Challenges;

public class ContractSelect : MonoBehaviour
{
    public GameObject frogButtons;
    public GameObject beetleButtons;
    public GameObject queenButtons;

    public GameObject description;

    public GameObject startButton;

    private GameObject currentButtons;

    [SerializeField] private FadeScreenHandler fadeScreen;

    protected virtual void Awake()
    {
        startButton.GetComponent<Button>().enabled = false;

        switch (ContractManager.Instance.SelectedLevel) 
        {
            case GameStateManager.FROG_SLIME_FIGHT:
                frogButtons.SetActive(true);
                currentButtons = frogButtons;
                break;
            case GameStateManager.BEETLE_FIGHT:
                beetleButtons.SetActive(true);
                currentButtons = beetleButtons;
                break;
            case GameStateManager.PRE_QUEEN_FIGHT:
                queenButtons.SetActive(true);
                currentButtons = queenButtons;
                break;
        }
        fadeScreen.SetDarkScreen();
        StartCoroutine(fadeScreen.FadeInLightScreen(1f));
    }

    public void OpenScene(string s) 
    {
        if (s != ContractManager.Instance.SelectedLevel) {
            ContractManager.Instance.SelectedLevel = ""; // "disengage contracts", semantic solution
        }

        StartCoroutine(FadeLevelIn(s));
    }

    public void StartLevel() 
    {
        OpenScene(ContractManager.Instance.SelectedLevel);
    }

    public void OnSelect(ContractButton button, bool select) {
        
        foreach (ContractButton other in currentButtons.GetComponentsInChildren<ContractButton>()) 
        {
            other.setSelected(false);
        }
        button.setSelected(select);

        // Default flavour text
        string flavourText = "Select a challenge to get started";

        if (!select)
        {
            flavourText = ContractManager.Instance.SetActiveChallenge(null);
            startButton.GetComponent<Button>().enabled = false;
        } 
        else 
        {
            startButton.GetComponent<Button>().enabled = true;
            switch (button.level)
            {
                case 0:
                    Debug.LogError("No Tutorial Contracts!");
                    break;
                case 1:
                    flavourText = ContractManager.Instance.SetActiveChallenge(
                        StringToChallenge<FrogChallenges>(button.contract)
                    );
                    break;
                case 2:
                    flavourText = ContractManager.Instance.SetActiveChallenge(
                        StringToChallenge<BeetleChallenges>(button.contract)
                    );
                    break;
                case 3:
                    flavourText = ContractManager.Instance.SetActiveChallenge(
                        StringToChallenge<QueenChallenges>(button.contract)
                    );
                    break;
            }
        }

        description.GetComponent<TMPro.TextMeshProUGUI>().text = flavourText;
    }

    T StringToChallenge<T>(string challenge) where T : Enum
    {
        return (T) Enum.Parse(typeof(T), challenge);
    }

    IEnumerator FadeLevelIn(string levelName)
    {
        yield return StartCoroutine(fadeScreen.FadeInDarkScreen(0.8f));
        SceneManager.LoadScene(levelName);
    }

}
