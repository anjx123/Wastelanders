using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEdit : MonoBehaviour
{
    private CardDatabase.WeaponType type;
    private void OnMouseDown()
    {
        DeckSelectionManager.Instance.WeaponDeckEdit(type);
    }

    public void SetType(CardDatabase.WeaponType type)
    {
        this.type = type;
    }
}
