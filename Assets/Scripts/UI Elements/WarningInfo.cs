
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class WarningInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text warningText;
    public UnityEngine.UI.Image popupImage;
    public bool popUpCanFade = false; //controls whether should fade
    public float secondTimer = 4f;

    public bool isActive = true; //controls whether active on screen

    public void ShowWarning(string text)
    {
        warningText.text = text;
        warningText.color = Color.black;
        popUpCanFade = true; //Warnings can fade out
        SetPopUpActive();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isActive)
        {
            popUpCanFade = false;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isActive)
        {
            popUpCanFade = true;
        }
    }

    public void DisplayDescription(string description)
    {
        warningText.text = description;
        warningText.color = Color.white;
        popUpCanFade = false; //Descriptions do not fade
        SetPopUpActive();
    }

    public void RemoveDescription()
    {
        warningText.text = "";
        SetPopUpInactive();
        popUpCanFade = true;
    }

    public void Update()
    {
        if (!popUpCanFade)
        {
            SetPopUpActive();
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
                    PopUpNotificationManager.Instance.isRunning = false; //Should probably be an event broadcast instead.
                    SetPopUpInactive();
                }
            }
        }
    }

    private void SetPopUpActive()
    {
        isActive = true;
        secondTimer = 4f;
        Color textColor = warningText.color;
        Color imageColor = popupImage.color;
        textColor.a = 1f;
        imageColor.a = 1f;
        warningText.color = textColor;
        popupImage.color = imageColor;
    }

    private void SetPopUpInactive()
    {
        isActive = false;
        Color textColor = warningText.color;
        Color imageColor = popupImage.color;
        textColor.a = 0;
        imageColor.a = 0;
        warningText.color = textColor;
        popupImage.color = imageColor;
    }
}