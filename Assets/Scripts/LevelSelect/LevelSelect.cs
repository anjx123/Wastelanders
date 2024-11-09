using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    public Button[] buttons;
    public GameObject levelButtons;
    public GameObject contractButtons;
    [SerializeField] private FadeScreenHandler fadeScreen;

    protected virtual void Awake()
    {
        ButtonArray();
        CheckContracts();
        fadeScreen.SetDarkScreen();
        StartCoroutine(fadeScreen.FadeInLightScreen(1f));
    }

    public void OpenScene(string s)
    {
        StartCoroutine(FadeLevelIn(s));
    }

    public void OpenContracts(string selectedLevel)
    {
        BountyManager.Instance.SelectedBounty = selectedLevel;
        StartCoroutine(FadeLevelIn(GameStateManager.CONTRACT_SELECT_NAME));
    }

    IEnumerator FadeLevelIn(string levelName)
    {
        yield return StartCoroutine(fadeScreen.FadeInDarkScreen(0.8f));
        SceneManager.LoadScene(levelName);
    }

    void ButtonArray()
    {
        int children = levelButtons.transform.childCount;
        buttons = new Button[children];
        for (int i = 0; i < children; i++)
        {
            GameObject gameObject = levelButtons.transform.GetChild(i).gameObject;
            buttons[i] = gameObject.GetComponent<Button>();
        }
    }

    // temporary solution to finding contract buttons and enabling them
    private readonly Dictionary<string, int> ButtonMap = new Dictionary<string, int>() {
        {GameStateManager.FROG_SLIME_FIGHT, 1},
        {GameStateManager.BEETLE_FIGHT, 2},
        {GameStateManager.PRE_QUEEN_FIGHT, 3},
    };
    public void CheckContracts() {
        // temp for testing
        GameStateManager.Instance.CompletedQueenFight = true;
        GameStateManager.Instance.CompletedBeetleFight = true;
        GameStateManager.Instance.CompletedFrogAndSlimeFight = true;

        void Disable(GameObject contractButton) {
            // can't make inactive, need to unrender so the positions remain
            contractButton.GetComponent<Button>().enabled = false;
            contractButton.GetComponent<CanvasRenderer>().SetAlpha(0);
            contractButton.transform.GetChild(0).GetComponent<CanvasRenderer>().SetAlpha(0);
        }

        // temporary hard code solution
        if (!GameStateManager.Instance.CompletedFrogAndSlimeFight) {
            Disable(contractButtons.transform.GetChild(ButtonMap[GameStateManager.FROG_SLIME_FIGHT]).gameObject);
        }
        if (!GameStateManager.Instance.CompletedBeetleFight) {
            Disable(contractButtons.transform.GetChild(ButtonMap[GameStateManager.BEETLE_FIGHT]).gameObject);
        }
        if (!GameStateManager.Instance.CompletedQueenFight) {
            Disable(contractButtons.transform.GetChild(ButtonMap[GameStateManager.PRE_QUEEN_FIGHT]).gameObject);
        }
    }
}
