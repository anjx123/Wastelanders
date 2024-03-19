using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffDeckEdit : CharacterSelect
{
    // Start is called before the first frame update
    public override void OnMouseDown()
    {
      DeckSelectionManager.Instance.WeaponDeckEdit(CardDatabase.WeaponType.STAFF);
    }

}
