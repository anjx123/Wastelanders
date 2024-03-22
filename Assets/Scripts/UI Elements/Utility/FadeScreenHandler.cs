using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScreenHandler : MonoBehaviour
{
    [SerializeField] private SpriteRenderer fadeScreen;

    private bool fadeActive = false;

    public void SetDarkScreen()
    {
        fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, 1f);
    }

    public IEnumerator FadeInLightScreen(float duration)
    {
        yield return StartCoroutine(FadeBackground(1f, 0f, duration));
    }

    public IEnumerator FadeInDarkScreen(float duration)
    {
        yield return StartCoroutine(FadeBackground(0f, 1f, duration));
    }

    //set (@param darkenScene) true to fade **combat background** in, false to fade out
    private IEnumerator FadeCombatBackground(bool darkenScene)
    {

        float startValue = fadeScreen.color.a;
        float duration = 1f;

        float endValue;
        if (darkenScene)
        {
            endValue = 0.8f;
        }
        else
        {
            startValue = Mathf.Max(fadeScreen.color.a - 0.3f, 0f); //Clamped to prevent visual nausua with strange alpha change
            endValue = 0f;
        }

        yield return StartCoroutine(FadeBackground(startValue, endValue, duration));
    }

    //Fade Background that gives you more control over the level of fade
    private IEnumerator FadeBackground(float startAlpha, float endAlpha, float duration)
    {
        if (fadeActive) yield break;
        fadeActive = true;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, newAlpha);
            yield return null;
        }
        fadeActive = false;
    }
}
