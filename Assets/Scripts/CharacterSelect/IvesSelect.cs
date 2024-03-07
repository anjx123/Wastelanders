using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IvesSelect : CharacterSelect
{
    [SerializeField] PlayerDatabase playerDatabase;

    public override void OnMouseDown()
    {
      DeckSelectionManager.Instance.CharacterChosen(playerDatabase.IvesData);
    }

}
