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
    public UnityEngine.UI.Image popupImage;
    public bool mouseHover = false;
    public float secondTimer = 4f;

    public void setText(string text)
    {
        warningText.text = text;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseHover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseHover = false;
    }

    public void Update()
    {
        if (mouseHover)
        {
            secondTimer = 4f;
            Color textColor = warningText.color;
            Color imageColor = popupImage.color;
            textColor.a = 1f;
            imageColor.a = 1f;
            warningText.color = textColor;
            popupImage.color = imageColor;
        }
        else
        {
            secondTimer -= Time.deltaTime;
            if (secondTimer < 0)
            {
                float disappearSpeed = 1f;
                Color textColor = warningText.color;
                Color imageColor = popupImage.color;
                textColor.a -= disappearSpeed * Time.deltaTime;
                imageColor.a -= disappearSpeed * Time.deltaTime;
                warningText.color = textColor;
                popupImage.color = imageColor;
                if (textColor.a < 0)
                {
                    PopUpNotificationManager.Instance.isRunning = false;
                    Destroy(gameObject);
                }
            }
        }
    }
}