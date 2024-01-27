using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarningInfo : MonoBehaviour
{
    public TMP_Text warningText;

    public void setText(string text)
    {
        warningText.text = text;
    }

    public void OnMouseOver()
    {
        Debug.Log("AAAAAAAAAAAAAA");
        PopUpNotificationManager.Instance.hover = true;
    }

    public void OnMouseExit()
    {
        Debug.Log("EEEEEEEEEEEEEE");
        PopUpNotificationManager.Instance.hover = false;
    }
}
