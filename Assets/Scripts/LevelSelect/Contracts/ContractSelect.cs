using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContractSelect : MonoBehaviour
{
    public GameObject frogButtons;
    public GameObject beetleButtons;
    public GameObject queenButtons;
    [SerializeField] private FadeScreenHandler fadeScreen;

    protected virtual void Awake()
    {
        switch (ContractManager.Instance.SelectedLevel) 
        {
            case GameStateManager.FROG_SLIME_FIGHT:
                frogButtons.SetActive(true);
                break;
            case GameStateManager.BEETLE_FIGHT:
                beetleButtons.SetActive(true);
                break;
            case GameStateManager.PRE_QUEEN_FIGHT:
                queenButtons.SetActive(true);
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

    IEnumerator FadeLevelIn(string levelName)
    {
        yield return StartCoroutine(fadeScreen.FadeInDarkScreen(0.8f));
        SceneManager.LoadScene(levelName);
    }

}
