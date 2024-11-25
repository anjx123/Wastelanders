using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponEdit : MonoBehaviour
{
    [SerializeField] private CardDatabase.WeaponType type;
    [SerializeField] private Color defaultColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color hoverColor = new Color(0.6f, 0.6f, 0.6f, 1f);
    private bool isMouseDown = false;
    public TMP_Text editText;
    private bool isLocked = false;

#nullable enable
    public delegate void WeaponEditDelegate(CardDatabase.WeaponType type);
    public static event WeaponEditDelegate? WeaponEditEvent;


    public void SetType(CardDatabase.WeaponType type)
    {
        this.type = type;
    }
    public void OnMouseDown()
    {
        if (isLocked) return;
        GetComponent<SpriteRenderer>().color = hoverColor;
        isMouseDown = true;
    }

    public void OnMouseUp()
    {
        if (isLocked) return;
        if (isMouseDown)
        {
            GetComponent<SpriteRenderer>().color = defaultColor;
            WeaponEditEvent?.Invoke(type);
        }
        isMouseDown = false;
    }

    public void OnMouseEnter()
    {
        if (isLocked) return;
        GetComponent<SpriteRenderer>().color = hoverColor;
    }

    public void OnMouseExit()
    {
        if (isLocked) return;
        GetComponent<SpriteRenderer>().color = defaultColor;
        isMouseDown = false;
    }

    public void SetText(string text)
    {
        editText.text = text;
    }

    public void SetLocked(bool isLocked)
    {
        this.isLocked = isLocked;
        SetText(isLocked ? "LOCKED" : "EDIT DECK");
    }
}
