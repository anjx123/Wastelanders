using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    public Button[] buttons;
    public GameObject levelButtons;

    protected virtual void Awake()
    {
        ButtonArray();
    }

    public void OpenScene(string s)
    {
        SceneManager.LoadScene(s);
    }

    void ButtonArray()
    {
        int children = levelButtons.transform.childCount;
        buttons = new Button[children];
        for (int i = 0; i < children; i++)
        {
            buttons[i] = levelButtons.transform.GetChild(i).gameObject.GetComponent<Button>();
        }
    }
}
