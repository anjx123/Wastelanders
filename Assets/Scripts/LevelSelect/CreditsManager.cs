using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{

    public FadeScreenHandler FadeScreenHandler;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RollCredits());
    }

    IEnumerator RollCredits()
    {

        FadeScreenHandler.SetDarkScreen();
        yield return StartCoroutine(FadeScreenHandler.FadeInLightScreen(2f));
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        yield return StartCoroutine(FadeScreenHandler.FadeInDarkScreen(1f));
        SceneManager.LoadScene(GameStateManager.MAIN_MENU_NAME);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
