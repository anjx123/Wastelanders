using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class WarningInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text warningText;
    public bool mouseHover = false;
    public float secondTImer = 5f;

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("aaaaaaa");
        mouseHover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("eeeeeeeeee");
        mouseHover = false;
    }

    public void Update()
    {
        if (mouseHover)
        {
            secondTImer = 5;
        }
        else
        {
            secondTImer -= Time.deltaTime;
            if (secondTImer <= 0)
            {
                PopUpNotificationManager.Instance.hover = false;
                //Destroy(this);
            }
        }
    }
}