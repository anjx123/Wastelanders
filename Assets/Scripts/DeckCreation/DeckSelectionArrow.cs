using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSelectionArrow : CharacterSelect
{
    public override void OnMouseDown()
    {
      DeckSelectionManager.Instance.PrevState();
    }

}
