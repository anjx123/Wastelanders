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
     Sets the CombatInfo sprite to the icon of this ActionClass
     */
    public void SetCombatSprite(ActionClass card)
    {
        animator.enabled = true;
        SpriteRenderer spriteRenderer = combatCardSprite.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = card.GetIcon();
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
            instance.transform.SetParent(buffList.transform, false);
            instance.GetComponent<SpriteRenderer>().sprite = buffs[str].GetIcon();
            instance.GetComponent<TextMeshProUGUI>().text = buffs[str].Stacks.ToString();
        }
    }
}
