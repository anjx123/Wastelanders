using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadNewScene()); 
    }

    IEnumerator LoadNewScene()
    {
        yield return new WaitForEndOfFrame();
        SceneManager.LoadScene(GameStateManager.nameOfRestartedLevel);
    }
}
