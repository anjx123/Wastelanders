using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using System.Linq;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] private SpriteRenderer characterPortrait;
    [SerializeField] private GameObject hover;
    [SerializeField] private GameObject lockIndicator;
    public PlayerDatabase.PlayerName playerName;
    private bool isMouseDown = false;
#nullable enable
    public delegate void CharacterSelectDelegate(PlayerDatabase.PlayerName playerName);
    public static event CharacterSelectDelegate? CharacterSelectedEvent;

    private bool isLocked = false;

    public void OnMouseDown()
    {
        if (isLocked) return;
        isMouseDown = true;
    }

    public void OnMouseUp()
    {
        if (isLocked) return;
        if (isMouseDown)
        {
            CharacterSelectedEvent?.Invoke(playerName);
        }
        isMouseDown = false;
    }

    public void OnMouseEnter()
    {
        if (isLocked) return;
        hover.SetActive(true);
        SetSpriteTransparency(characterPortrait, 1f);
    }

    public void OnMouseExit()
    {
        if (isLocked) return;
        isMouseDown = false;
        hover.SetActive(false);
        SetSpriteTransparency(characterPortrait, 0.75f);
    }

    public void SetSpriteTransparency(SpriteRenderer r, float newTransparency)
    {
        Color c = r.color;
        c.a = newTransparency;
        r.color = c;
    }

    public void SetLockedState(bool isLocked)
    {
        this.isLocked = isLocked;
        lockIndicator.SetActive(isLocked);
    }

}
