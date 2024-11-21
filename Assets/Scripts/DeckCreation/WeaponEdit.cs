using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponEdit : MonoBehaviour
{
    private CardDatabase.WeaponType type;
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
        GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.4f);
        isMouseDown = true;
    }

    public void OnMouseUp()
    {
        if (isLocked) return;
        if (isMouseDown)
        {
            GetComponent<SpriteRenderer>().color = Color.white;
            WeaponEditEvent?.Invoke(type);
        }
        isMouseDown = false;
    }

    public void OnMouseEnter()
    {
        if (isLocked) return;
        GetComponent<SpriteRenderer>().color = new Color(0.6f, 0.6f, 0.6f);
    }

    public void OnMouseExit()
    {
        if (isLocked) return;
        GetComponent<SpriteRenderer>().color = Color.white;
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
