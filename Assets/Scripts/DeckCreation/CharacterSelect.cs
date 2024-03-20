using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    public PlayerDatabase.PlayerName playerName;
    private bool isMouseDown = false;
#nullable enable
    public delegate void CharacterSelectDelegate(PlayerDatabase.PlayerName playerName);
    public static event CharacterSelectDelegate? CharacterSelectedEvent;

    public void OnMouseDown()
    {
        SetColor(new Color(0.4f, 0.4f, 0.4f));
        isMouseDown = true;
    }

    public void OnMouseUp()
    {
        if (isMouseDown)
        {
            SetColor(Color.white);
            CharacterSelectedEvent?.Invoke(playerName);
        }
        isMouseDown = false;
    }

    public void OnMouseEnter()
    {
        SetColor(new Color(0.6f, 0.6f, 0.6f));
    }

    public void OnMouseExit()
    {
        SetColor(Color.white);
        isMouseDown = false;
    }

    public void SetColor(Color newColor)
    {
        GetComponent<SpriteRenderer>().color = newColor;
        SpriteRenderer[] childSpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer spriteRenderer in childSpriteRenderers)
        {
            spriteRenderer.color = newColor;
        }
    }


}
