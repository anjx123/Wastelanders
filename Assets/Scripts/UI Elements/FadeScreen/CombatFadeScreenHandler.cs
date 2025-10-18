using System.Collections;
using UnityEngine;

public class CombatFadeScreenHandler : MonoBehaviour
{
    public static CombatFadeScreenHandler Instance { get; private set; }

    [SerializeField]
    private SpriteFadeHandler spriteFadeHandler;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
    }

    public string FADE_SORTING_LAYER => spriteFadeHandler.FadeSortingLayer;
    public int FADE_SORTING_LAYER_ID => spriteFadeHandler.FadeSortingLayerId;
    public int FADE_SORTING_ORDER => spriteFadeHandler.FadeSortingOrder;
    public float FADE_SCREEN_Z_VALUE => spriteFadeHandler.FadeScreenZValue;

    public void SetDarkScreen() => spriteFadeHandler.SetDarkScreen();

    public void SetLightScreen() => spriteFadeHandler.SetLightScreen();

    public IEnumerator FadeToAlpha(float targetAlpha, float duration)
    {
        yield return spriteFadeHandler.FadeToAlpha(targetAlpha, duration);
    }

    public IEnumerator FadeInLightScreen(float duration)
    {
        yield return spriteFadeHandler.FadeInLightScreen(duration);
    }

    public IEnumerator FadeInDarkScreen(float duration)
    {
        yield return spriteFadeHandler.FadeInDarkScreen(duration);
    }
}