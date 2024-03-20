using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSelectionArrow : MonoBehaviour
{
    private bool isMouseDown = false;

    public void OnMouseDown()
    {
        GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.4f);
        isMouseDown = true;
    }

    public void OnMouseUp()
    {
        if (isMouseDown)
        {
            GetComponent<SpriteRenderer>().color = Color.white;
            DeckSelectionManager.Instance.PrevState();
        }
        isMouseDown = false;
    }

    public void OnMouseEnter()
    {
        GetComponent<SpriteRenderer>().color = new Color(0.6f, 0.6f, 0.6f);
    }

    public void OnMouseExit()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
        isMouseDown = false;
    }

}
