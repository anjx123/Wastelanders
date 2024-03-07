using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackieSelect : CharacterSelect
{
    // Start is called before the first frame update
    [SerializeField] PlayerDatabase playerDatabase;
    

    public override void OnMouseDown()
    {
      DeckSelectionManager.Instance.CharacterChosen(playerDatabase.JackieData);
    }

}
