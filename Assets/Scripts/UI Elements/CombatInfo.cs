using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatInfo : MonoBehaviour
{
    public GameObject combatCardSprite;
    public Animator animator;
    public GameObject diceRollSprite;
    public List<Sprite> loadedSprites = new();

    /* 
     Ideally sets the rolled dice value to the sprite of the value, since thats not available it uses a default sprite.
     */
    public void SetDice(int value)
    {
        animator.enabled = false;
        diceRollSprite.GetComponent<SpriteRenderer>().sprite = loadedSprites[0];
    }

    /* 
     Sets the CombatInfo sprite to the icon of this ActionClass
     */
    public void SetCombatSprite(ActionClass card)
    {
        animator.enabled = true;
        SpriteRenderer spriteRenderer = combatCardSprite.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = card.GetIcon();
    }

    public void FaceLeft()
    {
        Transform parentTransform = this.transform;
        Vector3 flippedTransform = parentTransform.localScale;
        flippedTransform.x = -Mathf.Abs(flippedTransform.x);
        parentTransform.localScale = flippedTransform;
        diceRollSprite.GetComponent<SpriteRenderer>().flipX = true;
    }

    public void FaceRight()
    {
        Transform parentTransform = this.transform;
        Vector3 flippedTransform = parentTransform.localScale;
        flippedTransform.x = -Mathf.Abs(flippedTransform.x);
        parentTransform.localScale = flippedTransform;
        diceRollSprite.GetComponent<SpriteRenderer>().flipX = false;

    }
}
