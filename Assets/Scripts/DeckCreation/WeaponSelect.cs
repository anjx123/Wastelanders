using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardDatabase;

public class WeaponSelect : MonoBehaviour
{
    public CardDatabase.WeaponType type;
    [SerializeField] private bool hasSubFolders;
    [SerializeField] private GameObject checkmark;
    [SerializeField] private WeaponEdit weaponEdit;
    [SerializeField] private SpriteRenderer cardBodySprite;
    [SerializeField] private GameObject lockedIndicator;
    [SerializeField] private Color baseColor = Color.white;
    [SerializeField] private Color hoverColor = new Color(0.6f, 0.6f, 0.6f);

#nullable enable
    public delegate void WeaponSelectDelegate(WeaponSelect weaponSelect, CardDatabase.WeaponType type);
    public static event WeaponSelectDelegate? WeaponSelectEvent;

    private bool isMouseDown = false;
    private bool isLocked = false;

    private void Start()
    {
        weaponEdit.InitializeWeaponEdit(
            type,
            hasSubFolders,
            db => hasSubFolders ? db.GetDefaultSubFolderData(type) : db.GetCardsByType(type)
        );
    }


    public void SetSelected(bool isSelected)
    {
        checkmark.SetActive(isSelected);
    }

    public void OnMouseDown()
    {
        if (isLocked) return;
        SetColor(baseColor);
        isMouseDown = true;
    }

    public void OnMouseUp()
    {
        if (isLocked) return;
        if (isMouseDown)
        {
            SetColor(baseColor);
            WeaponSelectEvent?.Invoke(this, type);
        }
        isMouseDown = false;
    }

    public void OnMouseEnter()
    {
        if (isLocked) return;
        SetColor(hoverColor);
    }

    public void OnMouseExit()
    {
        if (isLocked) return;
        SetColor(baseColor);
        isMouseDown = false;
    }

    public void SetColor(Color newColor)
    {
        cardBodySprite.color = newColor;
    }

    public void SetLockedState(bool isLocked)
    {
        this.isLocked = isLocked;
        weaponEdit.SetLocked(isLocked);
        lockedIndicator.SetActive(isLocked);
    }

}
