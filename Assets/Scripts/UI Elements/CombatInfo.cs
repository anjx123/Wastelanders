using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatInfo : MonoBehaviour
{
    public GameObject combatCardSprite;
    public Animator animator;
    public GameObject diceRollSprite;
    public List<Sprite> loadedSprites = new();

    public void SetDice(int value)
    {
        animator.enabled = false;
        diceRollSprite.GetComponent<SpriteRenderer>().sprite = loadedSprites[0];
    }

    public void SetCombatSprite(ActionClass card)
    {
        animator.enabled = true;
        SpriteRenderer spriteRenderer = combatCardSprite.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = card.GetIcon();
    }
}
