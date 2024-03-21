using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Button restartButton;
    [SerializeField] Button levelSelectButton; 
    [SerializeField] GameObject gameOverText;

    void Start()
    {
        // Assign the click events to the buttons
        restartButton.onClick.AddListener(OnRestartClick);
        levelSelectButton.onClick.AddListener(OnLevelSelectClick);
        canvasGroup.alpha = 0f;
        FadeIn();

    }
    public void FadeIn()
    {
        StartCoroutine(FadeInCoroutine());
    }


    private float cycleScaling = 2f; // Higher the number, the faster one phase is 
    private float bobbingAmount = 0.1f; //Amplitude
    private float timer = 0;
    private float verticalOffset = 0;

    //Makes the game over text bob up and down!
    void Update()
    {
        float previousOffset = verticalOffset;
        float waveslice = Mathf.Sin(cycleScaling * timer);
        timer += Time.deltaTime;
        if (timer > Mathf.PI * 2)
        {
            timer = timer - (Mathf.PI * 2);
        }

        verticalOffset = waveslice * bobbingAmount;
        float translateChange = verticalOffset - previousOffset;
        gameOverText.transform.position = new Vector3(gameOverText.transform.position.x, gameOverText.transform.position.y + translateChange, gameOverText.transform.position.z);
    }

    IEnumerator FadeInCoroutine()
    {
        float duration = 1f; // Duration of the fade in seconds
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            canvasGroup.alpha = t;
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    private void OnRestartClick()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }

    private void OnLevelSelectClick()
    {
        SceneManager.LoadScene("LevelSelect");
    }
}
