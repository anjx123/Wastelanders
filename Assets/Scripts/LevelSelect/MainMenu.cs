using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject wastelandersText;

    [SerializeField] private Button quitButton;
    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    private void Start()
    {
        #if UNITY_WEBGL
            quitButton.gameObject.SetActive(false);
        #endif
    }


    private readonly float cycleScaling = 2f; // Higher the number, the faster one phase is 
    private readonly float bobbingAmount = 0.1f; //Amplitude
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
        wastelandersText.transform.position = new Vector3(wastelandersText.transform.position.x, wastelandersText.transform.position.y + translateChange, wastelandersText.transform.position.z);
    }
}
