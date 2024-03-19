using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelect : MonoBehaviour
{
    public CardDatabase.WeaponType type;
    [SerializeField] private GameObject checkmark;
    [SerializeField] private WeaponEdit weaponEdit;

    private void Start()
    {
        weaponEdit.SetType(type);
    }
    private void OnMouseDown()
    {
        DeckSelectionManager.Instance.WeaponSelected(this, type);
    }

    public void SetSelected(bool isSelected)
    {
        checkmark.SetActive(isSelected);
    }
}
