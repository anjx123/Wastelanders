using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EvolveInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text warningText;
    public UnityEngine.UI.Image popupImage;
    public UnityEngine.UI.Image cardImage;
    public bool popUpCanFade = false; //controls whether should fade
    public float secondTimer = 4f;
    private Vector2 initialPosition = new Vector2(120, 120);
    private Vector2 finalPosition = new Vector2(120, -80);
    private float slideDuration = 1.5f;

    public RectTransform rectTransform;

    public bool isActive = true; //controls whether active on screen

    void Start()
    {
        SetPopUpInactive();
        Canvas[] canvases = GetComponentsInChildren<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            if (canvas != GetComponent<Canvas>())
            {
                rectTransform = canvas.GetComponent<RectTransform>();
                Debug.Log("Inner Canvas RectTransform assigned.");
                break;
            }
        }
    }

    public IEnumerator ShowEvolve(Sprite spr,  string text)
    {
        warningText.text = text;
        cardImage.sprite = spr;
        warningText.color = Color.white;
        popUpCanFade = false;
        SetPopUpActive();

        Debug.Log(rectTransform.position);
        yield return StartCoroutine(MoveToPosition());
        popUpCanFade = true;
    }

    private IEnumerator MoveToPosition()
    {
        Debug.Log("start slide");
        float elapsedTime = 0f;
        while (elapsedTime < slideDuration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(initialPosition, finalPosition, elapsedTime / slideDuration);     
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rectTransform.anchoredPosition = finalPosition;
        Debug.Log("finish slide");

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
        Color cardColor = cardImage.color;
        textColor.a = 1f;
        imageColor.a = 1f;
        cardColor.a = 1f;
        warningText.color = textColor;
        popupImage.color = imageColor;
        cardImage.color = cardColor;
    }

    private void SetPopUpInactive()
    {
        isActive = false;
        Color textColor = warningText.color;
        Color imageColor = popupImage.color;
        Color cardColor = cardImage.color;
        textColor.a = 0;
        imageColor.a = 0;
        cardColor.a = 0;
        warningText.color = textColor;
        popupImage.color = imageColor;
        cardImage.color = cardColor;
        rectTransform.anchoredPosition = initialPosition;
    }
}
