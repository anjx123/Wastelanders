using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FadeScreenHandler : MonoBehaviour
{
    [SerializeField] private SpriteRenderer fadeScreen;

    private Coroutine _animationCoroutine = null;
    private float _intendedAlpha = 0.0f;

    void Awake()
    {
        if (fadeScreen == null)
        {
            fadeScreen = GetComponent<SpriteRenderer>();
        }

        if (fadeScreen == null)
        {
            Debug.LogError("FadeScreenHandler requires a SpriteRenderer component or assignment.", this);
            enabled = false;
            return;
        }
    }

    public void SetDarkScreen()
    {
        StopCurrentFadeAnimation(); 
        SetAlpha(1f);
        _intendedAlpha = 1f; 
    }

    public void SetLightScreen() 
    {
        StopCurrentFadeAnimation();
        SetAlpha(0f);
        _intendedAlpha = 0f;
    }

    public IEnumerator FadeInLightScreen(float duration)
    {
        RequestFade(0f, duration);
        return WaitForCurrentFadeCompletion();
    }

    public IEnumerator FadeInDarkScreen(float duration)
    {
        RequestFade(1f, duration);
        return WaitForCurrentFadeCompletion();
    }

    private void RequestFade(float targetAlpha, float duration)
    {
        _intendedAlpha = Mathf.Clamp01(targetAlpha); 
        float effectiveDuration = Mathf.Max(duration, 0f); 

        StopCurrentFadeAnimation();
        _animationCoroutine = StartCoroutine(AnimateFade(fadeScreen.color.a, _intendedAlpha, effectiveDuration));

    }

    private IEnumerator WaitForCurrentFadeCompletion()
    {
        yield return new WaitUntil(() => _animationCoroutine == null);
    }

    private IEnumerator AnimateFade(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float fraction = elapsedTime / duration; 
            float newAlpha = Mathf.LerpUnclamped(startAlpha, endAlpha, fraction);
            SetAlpha(Mathf.Clamp01(newAlpha));

            yield return null;
        }


        SetAlpha(endAlpha);
        _animationCoroutine = null;
    }

    private void SetAlpha(float alpha)
    {
        if (fadeScreen != null)
        {
            Color currentColor = fadeScreen.color;
            currentColor.a = Mathf.Clamp01(alpha); 
            fadeScreen.color = currentColor;
        }
    }

    private void StopCurrentFadeAnimation()
    {
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
            _animationCoroutine = null;
        }
    }
}