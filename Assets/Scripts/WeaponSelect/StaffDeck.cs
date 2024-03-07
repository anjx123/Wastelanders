using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STAFFDeck : CharacterSelect
{
    // Start is called before the first frame update
    public override void OnMouseDown()
    {
      DeckSelectionManager.Instance.WeaponSelected(CardDatabase.WeaponType.STAFF);
    }

}
