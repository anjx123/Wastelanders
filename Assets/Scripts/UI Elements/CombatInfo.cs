using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatInfo : MonoBehaviour
{
    public GameObject combatCardSprite;
    public HorizontalLayoutGroup buffList;
    public Animator animator;
    public GameObject diceRollSprite;
    public List<Sprite> loadedSprites = new();

    public GameObject buffIconPrefab;


    /* 
     Ideally sets the rolled dice value to the sprite of the value, since thats not available it uses a default sprite.
     */
    public void SetDice(int value)
    {
        animator.enabled = false;
        diceRollSprite.GetComponent<SpriteRenderer>().sprite = loadedSprites[0];
    }

    /* 
     Sets the CombatInfo sprite to the icon of this ActionClass.
    Pass in null to discard the current sprite.
     */
#nullable enable
    public void SetCombatSprite(ActionClass? card)
    {
        animator.enabled = true;
        SpriteRenderer spriteRenderer = combatCardSprite.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = card?.GetIcon();
    }
#nullable disable
    //Flips the CombatInfo so that the Icon is on the Right of the entity
    public void FaceLeft()
    {
        Transform parentTransform = this.transform;
        Vector3 flippedTransform = parentTransform.localScale;
        flippedTransform.x = -Mathf.Abs(flippedTransform.x);
        parentTransform.localScale = flippedTransform;
        diceRollSprite.GetComponent<SpriteRenderer>().flipX = true;

        CombatManager.Instance.UpdateCameraBounds(); //Bad placement here
    }
    //Flips the CombatInfo so that the Icon is on the LEFT of the entity
    public void FaceRight()
    {
        Transform parentTransform = this.transform;
        Vector3 flippedTransform = parentTransform.localScale;
        flippedTransform.x = Mathf.Abs(flippedTransform.x);
        parentTransform.localScale = flippedTransform;
        diceRollSprite.GetComponent<SpriteRenderer>().flipX = false;

        CombatManager.Instance.UpdateCameraBounds(); //Bad placement here, but I cant think of where else id put it

    }

    //A Cheat implementation that relies on the implementation of FaceRight/Left 
    public bool IsFacingRight()
    {
        return transform.localScale.x > 0;
    }
    public void UpdateBuffs(Dictionary<string, StatusEffect> buffs)
    {
        foreach (Transform child in buffList.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (string str in buffs.Keys)
        {
            GameObject instance = Instantiate(buffIconPrefab);
            BuffIcons buffIcon = instance.GetComponent<BuffIcons>();
            buffIcon.transform.SetParent(buffList.transform, false);
            buffIcon.SetIcon(buffs[str].GetIcon());
            buffIcon.SetText(buffs[str].Stacks.ToString());
        }
    }
}
