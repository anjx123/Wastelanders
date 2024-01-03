using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffIcons : MonoBehaviour
{
    public Image buffIcon;
    public TextMeshProUGUI textMeshProUGUI;

    public void SetText(string text)
    {
        textMeshProUGUI.text = text;
    }

    public void SetIcon(Sprite icon)
    {
        buffIcon.sprite = icon;
    }

}
