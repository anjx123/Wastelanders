using System.Collections;
using UnityEngine;

public class UIFadeScreenManager : PersistentSingleton<UIFadeScreenManager>
{
    [SerializeField]
    private UIFadeHandler uiFadeHandler;
    
    public void SetDarkScreen() => uiFadeHandler.SetDarkScreen();

    public void SetLightScreen() => uiFadeHandler.SetLightScreen();
   
    public IEnumerator FadeToAlpha(float targetAlpha, float duration)
    {
        yield return uiFadeHandler.FadeToAlpha(targetAlpha, duration);
    }

    public IEnumerator FadeInLightScreen(float duration)
    {
        yield return uiFadeHandler.FadeInLightScreen(duration);
    }

    public IEnumerator FadeInDarkScreen(float duration)
    {
        yield return uiFadeHandler.FadeInDarkScreen(duration);
    }
}