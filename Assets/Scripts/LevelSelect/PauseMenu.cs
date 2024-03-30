using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool IsPaused = false;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private Canvas pauseCanvas;

    public delegate void PauseMenuEventHandler();
    public static event PauseMenuEventHandler onPauseMenuActivate;   // for more complex functions that cannot use isPaused
    public static event PauseMenuEventHandler onPauseMenuDeactivate;


    private void Start()
    {
        pauseCanvas.sortingLayerName = "Top";
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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
    }

    public void LevelSelectScene()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(GameStateManager.LEVEL_SELECT_NAME);
    }


    public void DeckSelectClick()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(GameStateManager.SELECTION_SCREEN_NAME);
    }

    public void ResetScene()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        GameStateManager.Restart(SceneManager.GetActiveScene().name);
    }
}
