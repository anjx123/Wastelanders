using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponEdit : MonoBehaviour
{
    private CardDatabase.WeaponType type;
    private bool isMouseDown = false;

#nullable enable
    public delegate void WeaponEditDelegate(CardDatabase.WeaponType type);
    public static event WeaponEditDelegate? WeaponEditEvent;


    public void SetType(CardDatabase.WeaponType type)
    {
        this.type = type;
    }
    public void OnMouseDown()
    {
        GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.4f);
        isMouseDown = true;
    }

    public void OnMouseUp()
    {
        if (isMouseDown)
        {
            GetComponent<SpriteRenderer>().color = Color.white;
            WeaponEditEvent?.Invoke(type);
        }
        isMouseDown = false;
    }

    public void OnMouseEnter()
    {
        GetComponent<SpriteRenderer>().color = new Color(0.6f, 0.6f, 0.6f);
    }

    public void OnMouseExit()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
        isMouseDown = false;
    }
}
