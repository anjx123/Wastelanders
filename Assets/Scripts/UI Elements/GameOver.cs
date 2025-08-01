using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class GameOver : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Button restartButton;
    [SerializeField] bool shouldJumpToCombatWhenRestart = true;
    [SerializeField] Button levelSelectButton;
    [SerializeField] Button deckSelectButton;
    [SerializeField] GameObject gameOverText;
    public const float FADE_IN_TIME = 1f;

    void OnEnable()
    {
        // Assign the click events to the buttons
        restartButton.onClick.AddListener(() => StartCoroutine(OnRestartClick()));
        levelSelectButton.onClick.AddListener(() => StartCoroutine(OnLevelSelectClick()));
        deckSelectButton.onClick.AddListener(() => StartCoroutine(OnDeckSelectClick()));
        canvasGroup.alpha = 0f;
        FadeIn();
        // if this pops up then the game is over AND this takes care of the fade thing too.
        AudioManager.Instance.PlayDeath();
    }
    void OnDisable()
    {
        // Unsubscribe the click events from the buttons
        restartButton.onClick.RemoveListener(() => StartCoroutine(OnRestartClick()));
        levelSelectButton.onClick.RemoveListener(() => StartCoroutine(OnLevelSelectClick()));
        deckSelectButton.onClick.RemoveListener(() => StartCoroutine(OnDeckSelectClick()));
    }


    public void FadeIn()
    {
        StartCoroutine(FadeCoroutine(true, FADE_IN_TIME));
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

    IEnumerator FadeCoroutine(bool isFadingIn, float duration)
    {
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            canvasGroup.alpha = isFadingIn ? t : 1f - t;
            yield return null;
        }

        canvasGroup.alpha = isFadingIn ? 1f : 0f;
    }


    //If your game over isnt registering clicks, check the evenSystem in the hiearchy to see where your clicks are actually landing on 
    private IEnumerator OnRestartClick()
    {
        yield return StartCoroutine(FadeCoroutine(false, 0.7f));
        GameStateManager.Instance.JumpToCombat = shouldJumpToCombatWhenRestart;
        GameStateManager.Instance.Restart();
    }

    private IEnumerator OnLevelSelectClick()
    {
        yield return StartCoroutine(FadeCoroutine(false, 1f));
        GameStateManager.Instance.LoadScene(SceneData.Get<SceneData.LevelSelect>().SceneName);
    }

    private IEnumerator OnDeckSelectClick()
    {
        yield return StartCoroutine(FadeCoroutine(false, 1f));
        GameStateManager.Instance.LoadScene(SceneData.Get<SceneData.SelectionScreen>().SceneName);
    }
}
