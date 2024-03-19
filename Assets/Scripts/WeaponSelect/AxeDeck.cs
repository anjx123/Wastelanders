using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeDeck : CharacterSelect
{
    // Start is called before the first frame update
    public override void OnMouseDown()
    {
      DeckSelectionManager.Instance.WeaponSelected(this, CardDatabase.WeaponType.AXE);
    }

}
