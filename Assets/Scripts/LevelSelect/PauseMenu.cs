using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool IsPaused = false;
    public static bool ScreenShake = true;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private Canvas pauseCanvas;

    [SerializeField] private GameObject glossary;
    [SerializeField] private GameObject history;
    [SerializeField] private GameObject buttonparent;


    [SerializeField] private TextMeshProUGUI historyText;

    [SerializeField]
    private Slider musicSlider, sfxSlider; 


    public delegate void PauseMenuEventHandler();
    public static event PauseMenuEventHandler onPauseMenuActivate;   // for more complex functions that cannot use isPaused
    public static event PauseMenuEventHandler onPauseMenuDeactivate;


    bool isDialogueHistoryShowing = false;
    public bool DialogueHistoryState
    {
        get { return isDialogueHistoryShowing; }
        set
        {
            if (value == true)
            {
                ShowHistory();
            }
            else
            {
                HideHistory();
            }
            isDialogueHistoryShowing = value;
        }
    }

    void ShowHistory()
    {
        RenderHistory();
        history.SetActive(true);
        buttonparent.SetActive(false);
    }

    void HideHistory()
    {
        history.SetActive(false);   
        buttonparent.SetActive(true);
    }

    void RenderHistory()
    {
        List<DialogueText> list = DialogueManager.Instance?.GetHistory();
        historyText.text = "";
        foreach (DialogueText dialogue in list)
        {
            historyText.text += "<b>" + dialogue.SpeakerName + "</b>" + "\n" + dialogue.BodyText + "\n\n";
        }
    }

    public void SetHistoryState(bool state)
    {
        DialogueHistoryState = state;
    }

    bool isGlossaryShowing = false;
    public bool GlossaryState { get { return isGlossaryShowing; }
        set
        {
            if (value == true)
            {
                ShowGlossary();
            } else
            {
                HideGlossary();
            }
            isGlossaryShowing = value;
        }
    }

    void ShowGlossary()
    {
        glossary.SetActive(true);
        buttonparent.SetActive(false);
    }

    void HideGlossary()
    {
        glossary.SetActive(false);
        buttonparent.SetActive(true);
    }

    public void SetGlossary(bool isShowing)
    {
        GlossaryState = isShowing;
    }


    private void Start()
    {
        musicSlider.value = 0.7f;
        sfxSlider.value = 0.7f;
        AudioManager.Instance.MusicVolume(musicSlider.value);
        AudioManager.Instance.SFXVolume(sfxSlider.value);
        pauseCanvas.sortingLayerName = "Top";
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
            {
                ResumeScene();
            } else
            {
                PauseScene();
            }
        }
    }




    public void ResumeScene()
    {
        pauseMenuPanel.SetActive(false);
        onPauseMenuDeactivate?.Invoke();
        Time.timeScale = 1f;
        IsPaused = false;
    }

    public void PauseScene()
    {
        pauseMenuPanel.SetActive(true);
        onPauseMenuActivate?.Invoke();
        Time.timeScale = 0f;
        IsPaused = true;

        if (isDialogueHistoryShowing)
        {
            RenderHistory();
        }
    }

    public void LevelSelectScene()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        GameStateManager.Instance.LoadScene(SceneData.Get<SceneData.LevelSelect>().SceneName);
    }


    public void DeckSelectClick()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        GameStateManager.Instance.LoadScene(SceneData.Get<SceneData.SelectionScreen>().SceneName);
    }

    public void ResetScene()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        GameStateManager.Instance.Restart();
    }

    public void ToggleMusic()
    {
        AudioManager.Instance.ToggleMusic();
    }

    public void ToggleSFX()
    {
        AudioManager.Instance.ToggleSFX();
    }

    public void MusicVolume()
    {
        AudioManager.Instance.MusicVolume(musicSlider.value);
    }

    public void SFXVolume()
    {
        AudioManager.Instance.SFXVolume(sfxSlider.value);
    }

    public void ToggleScreenShake()
    {
        PauseMenu.ScreenShake = !PauseMenu.ScreenShake;
    }
}
