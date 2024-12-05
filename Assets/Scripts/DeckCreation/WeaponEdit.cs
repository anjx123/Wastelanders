using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static CardDatabase;
using static ISubWeaponType;
using static WeaponEditInformation;

public class WeaponEdit : MonoBehaviour
{
    [SerializeField] private Color baseColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color hoverColor = new Color(0.6f, 0.6f, 0.6f, 1f);
    private bool isMouseDown = false;
    public TMP_Text editText;
    private bool isLocked = false;
    private WeaponEditInformation WeaponEditInformation { get; set; }

#nullable enable
    public delegate void WeaponEditDelegate(WeaponEditInformation weaponEditInformation);
    public static event WeaponEditDelegate? WeaponEditEvent;


    public void InitializeWeaponEdit(WeaponType type, bool showSubFolders, GetRenderableCards getCards)
    {
        WeaponEditInformation = new WeaponEditInformation(type, showSubFolders, getCards);
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
            GetComponent<SpriteRenderer>().color = baseColor;
            WeaponEditEvent?.Invoke(WeaponEditInformation);
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
        GetComponent<SpriteRenderer>().color = baseColor;
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

public record WeaponEditInformation
{
    public WeaponType WeaponType { get; }
    public bool ShowSubFolders { get; }
    public GetRenderableCards GetCards { get; }

    public delegate List<ActionClass> GetRenderableCards(CardDatabase cardDatabase);

    public WeaponEditInformation(CardDatabase.WeaponType weaponType, bool showSubFolders, GetRenderableCards getCards)
    {
        WeaponType = weaponType;
        ShowSubFolders = showSubFolders;
        GetCards = getCards;
    }
}
