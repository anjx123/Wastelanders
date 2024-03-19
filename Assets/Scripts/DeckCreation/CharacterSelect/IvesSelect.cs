using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IvesSelect : CharacterSelect
{
    [SerializeField] PlayerDatabase playerDatabase;

    public override void OnMouseDown()
    {
      DeckSelectionManager.Instance.CharacterChosen(playerDatabase.IvesData);
        GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.4f);
    }

    public void OnMouseEnter()
    {
        GetComponent<SpriteRenderer>().color = new Color(0.6f, 0.6f, 0.6f);
    }

    public void OnMouseExit()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
    }

}
