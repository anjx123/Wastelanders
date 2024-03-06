using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolDeck : CharacterSelect
{
    // Start is called before the first frame update
    public override void OnMouseDown()
    {
      WeaponSelected(CardDatabase.WeaponType.PISTOL)
    }

}
