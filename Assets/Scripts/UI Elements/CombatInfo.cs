using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatInfo : MonoBehaviour
{
    public GameObject combatCardSprite;
    public Animator animator;
    public GameObject diceRollSprite;
    public List<Sprite> loadedSprites = new();

    public void setDice(int value)
    {
        animator.speed = 0;
        combatCardSprite.GetComponent<SpriteRenderer>().sprite = loadedSprites[0];
        animator.speed = 1;
    }

    public void setCombatSprite(ActionClass card)
    {
        SpriteRenderer spriteRenderer = combatCardSprite.GetComponent<SpriteRenderer>();
        GameObject cardObject = card.gameObject;

        spriteRenderer.sprite = cardObject.GetComponent<SpriteRenderer>().sprite;
    }
}
