using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private RectTransform myRectTransform;
    public Slider slider;
    public TMP_Text healthText;
    public Canvas healthCanvas;

    public void Start()
    {
        healthCanvas.sortingLayerName = CombatManager.Instance.FADE_SORTING_LAYER;
        DeEmphasize();
    }

    public void setMaxHealth(int maxHealth)
    {
        slider.maxValue = maxHealth;
    }

    public void setHealth(int health)
    {
        slider.value = health;
        healthText.text = health.ToString();
    }

    public void Emphasize()
    {
        Vector3 myPosition = myRectTransform.localPosition;
        myPosition.z = CombatManager.Instance.FADE_SCREEN_Z_VALUE;
        myRectTransform.localPosition = myPosition;
    }
    public void DeEmphasize()
    {
        Vector3 myPosition = myRectTransform.localPosition;
        myPosition.z = CombatManager.Instance.FADE_SCREEN_Z_VALUE + 4;
        myRectTransform.localPosition = myPosition;

    }
}
