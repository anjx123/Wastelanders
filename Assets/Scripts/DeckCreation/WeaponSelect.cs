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
    private bool isLocked = false;

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
        if (isLocked) return;
        SetColor(new Color(0.4f, 0.4f, 0.4f));
        isMouseDown = true;
    }

    public void OnMouseUp()
    {
        if (isLocked) return;
        if (isMouseDown)
        {
            SetColor(Color.white);
            WeaponSelectEvent?.Invoke(this, type);
        }
        isMouseDown = false;
    }

    public void OnMouseEnter()
    {
        if (isLocked) return;
        SetColor(new Color(0.6f, 0.6f, 0.6f));
    }

    public void OnMouseExit()
    {
        if (isLocked) return;
        SetColor(Color.white);
        isMouseDown = false;
    }

    public void SetColor(Color newColor)
    {
        GetComponent<SpriteRenderer>().color = newColor;
    }

    public void SetLockedState(bool isLocked)
    {
        if (isLocked)
        {
            this.isLocked = true;
            SetColor(Color.grey);
            weaponEdit.SetLocked(true);
            weaponEdit.SetText("Locked");
        }
        else
        {
            this.isLocked = false;
            SetColor(Color.grey);
            weaponEdit.SetLocked(false);
            weaponEdit.SetText("Edit");
        }
    }

}
