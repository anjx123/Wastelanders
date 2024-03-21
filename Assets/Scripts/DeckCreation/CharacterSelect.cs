using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class CharacterSelect : MonoBehaviour
{
    public PlayerDatabase.PlayerName playerName;
    private bool isMouseDown = false;
#nullable enable
    public delegate void CharacterSelectDelegate(PlayerDatabase.PlayerName playerName);
    public static event CharacterSelectDelegate? CharacterSelectedEvent;
    public TMP_Text editText;
    private bool isLocked = false;

    public void OnMouseDown()
    {
        if (isLocked) return;
        SetColor(new Color(0.4f, 0.4f, 0.4f));
        isMouseDown = true;
    }

    public void OnMouseUp()
    {
        if (isLocked) return;
        if (isMouseDown)
        {
            SetColor(Color.white);
            CharacterSelectedEvent?.Invoke(playerName);
        }
        isMouseDown = false;
    }

    public void OnMouseEnter()
    {
        if (isLocked) return;
        SetColor(new Color(0.6f, 0.6f, 0.6f));
    }

    public void OnMouseExit()
    {
        if (isLocked) return;
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

    public void SetLockedState(bool isLocked) {
            if (isLocked)
            {
                this.isLocked = true;
                SetColor(Color.grey);
                editText.text = "Locked";
            } else
            {
                this.isLocked = false;
                SetColor(Color.grey);
                editText.text = "Edit";
            }
        }


}
