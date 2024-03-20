using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelect : MonoBehaviour
{
    public CardDatabase.WeaponType type;
    [SerializeField] private GameObject checkmark;
    [SerializeField] private WeaponEdit weaponEdit;

#nullable enable
    public delegate void WeaponSelectDelegate(WeaponSelect weaponSelect, CardDatabase.WeaponType type);
    public static event WeaponSelectDelegate? WeaponSelectEvent;

    private bool isMouseDown = false;

    private void Start()
    {
        weaponEdit.SetType(type);
    }

    public void SetSelected(bool isSelected)
    {
        checkmark.SetActive(isSelected);
    }

    public void OnMouseDown()
    {
        SetColor(new Color(0.4f, 0.4f, 0.4f));
        isMouseDown = true;
    }

    public void OnMouseUp()
    {
        if (isMouseDown)
        {
            SetColor(Color.white);
            WeaponSelectEvent?.Invoke(this, type);
        }
        isMouseDown = false;
    }

    public void OnMouseEnter()
    {
        SetColor(new Color(0.6f, 0.6f, 0.6f));
    }

    public void OnMouseExit()
    {
        SetColor(Color.white);
        isMouseDown = false;
    }

    public void SetColor(Color newColor)
    {
        GetComponent<SpriteRenderer>().color = newColor;
    }

}
