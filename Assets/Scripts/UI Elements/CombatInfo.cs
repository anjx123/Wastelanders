using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatInfo : MonoBehaviour
{
    public GameObject combatCardSprite;
    public HorizontalLayoutGroup buffList;
    public Animator diceAnimator;
    public GameObject diceRollSprite;
    public List<Sprite> loadedSprites = new();
    public GameObject diceRollText;
    public GameObject buffIconPrefab;
    public HealthBar healthBar;
    public GameObject crosshair;

    private float ROTATION_SPEED = 30f;

    private Canvas buffListCanvas;

    public void Start()
    {
        diceRollText.GetComponent<MeshRenderer>().sortingOrder = diceRollSprite.GetComponent<SpriteRenderer>().sortingOrder + 1;
        buffListCanvas = buffList.gameObject.GetComponent<Canvas>();
        buffListCanvas.overrideSorting = true;
        buffListCanvas.sortingLayerName = CombatManager.Instance.FADE_SORTING_LAYER;
        diceRollText.GetComponent<MeshRenderer>().sortingLayerName = CombatManager.Instance.FADE_SORTING_LAYER;
    }

    public void Update()
    {
        crosshair.transform.Rotate(Vector3.forward * ROTATION_SPEED * Time.deltaTime);
    }

    /* 
     Ideally sets the rolled dice value to the sprite of the value, since thats not available it uses a default sprite.
     */
    public void SetDice(int value)
    {
        diceAnimator.enabled = false;
        diceRollSprite.GetComponent<SpriteRenderer>().sprite = loadedSprites[0];
        diceRollText.GetComponent<TMP_Text>().text = value.ToString();
    }

    // Sets the appropriate color of the dice
    public void setDiceColor(bool won)
    {
        if (won)
        {
            diceRollText.GetComponent<TMP_Text>().color = Color.green;
        }
        else
        {
            diceRollText.GetComponent<TMP_Text>().color = Color.red;
        }
    }

    /* 
     Sets the CombatInfo sprite to the icon of this ActionClass.
    Pass in null to discard the current sprite.
     */
    public void SetCombatSprite(ActionClass card)
    {
        diceAnimator.enabled = true;
        SpriteRenderer spriteRenderer = combatCardSprite.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = card.GetIcon();
        diceRollText.GetComponent<TextMeshPro>().text = null;
    }

    public void EnableDice()
    {
        diceAnimator.enabled = true;
        diceRollSprite.GetComponent<SpriteRenderer>().enabled = true;
        diceRollText.GetComponent<TMP_Text>().enabled = true;
    }

    public void DisableDice()
    {
        diceAnimator.enabled = false;
        diceRollSprite.GetComponent<SpriteRenderer>().enabled = false;
        diceRollText.GetComponent<TMP_Text>().enabled = false;
        diceRollText.GetComponent<TextMeshPro>().text = null;
    }

    public void DeactivateCombatSprite()
    {
        SpriteRenderer spriteRenderer = combatCardSprite.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = null;
    }

    public void ActivateCrosshair()
    {
        if (!crosshair.activeSelf)
        {
            crosshair.SetActive(true);
        }
    }

    public void DeactivateCrosshair()
    {
        if (crosshair.activeSelf)
        {
            crosshair.SetActive(false);
        }
    }
    public void Emphasize()
    {
        combatCardSprite.GetComponent<SpriteRenderer>().sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER + 1;
        diceRollSprite.GetComponent<SpriteRenderer>().sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER + 1;
        buffListCanvas.sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER + 1;
        diceRollText.GetComponent<MeshRenderer>().sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER + 1;
        healthBar.Emphasize();
    }
    public void DeEmphasize()
    {
        combatCardSprite.GetComponent<SpriteRenderer>().sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER - 1;
        diceRollSprite.GetComponent<SpriteRenderer>().sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER - 1;
        buffListCanvas.sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER - 1;
        diceRollText.GetComponent<MeshRenderer>().sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER - 1;
        healthBar.DeEmphasize();
    }

    public void SetMaxHealth(int maxHealth)
    {
        healthBar.setMaxHealth(maxHealth);
    }

    public void SetHealth(int health)
    {
        healthBar.setHealth(health);
    }
    //Flips the CombatInfo so that the Icon is on the Right of the entity
    public void FaceLeft()
    {

        FlipTransform(this.transform, false);
        FlipTransform(diceRollText.transform, false);
        FlipTransform(healthBar.transform, false);
        foreach (Transform child in buffList.transform)
        {
            FlipTransform(child, false);
        }
        diceRollSprite.GetComponent<SpriteRenderer>().flipX = true;

        
        

        CombatManager.Instance.UpdateCameraBounds(); //Bad placement here
    }
    //Flips the CombatInfo so that the Icon is on the LEFT of the entity
    public void FaceRight()
    {
        FlipTransform(this.transform, true);
        FlipTransform(diceRollText.transform, true);
        FlipTransform(healthBar.transform, true);
        foreach (Transform child in buffList.transform)
        {
            FlipTransform(child, true);
        }
        diceRollSprite.GetComponent<SpriteRenderer>().flipX = false;

        CombatManager.Instance.UpdateCameraBounds(); //Bad placement here, but I cant think of where else id put it

    }

    public void FlipTransform(Transform transform, bool faceRight)
    {
        if (faceRight) //Face Right
        {
            Vector3 flippedTransform = transform.localScale;
            flippedTransform.x = Mathf.Abs(flippedTransform.x);
            transform.localScale = flippedTransform;
        }
        else
        {
            Vector3 flippedTransform = transform.localScale;
            flippedTransform.x = -Mathf.Abs(flippedTransform.x);
            transform.localScale = flippedTransform;
        }
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
            if (buffs[str].Stacks == 0) continue;

            GameObject instance = Instantiate(buffIconPrefab);
            BuffIcons buffIcon = instance.GetComponent<BuffIcons>();
            buffIcon.transform.SetParent(buffList.transform, false);
            buffIcon.SetIcon(buffs[str].GetIcon());
            buffIcon.SetText(buffs[str].Stacks.ToString());
            if (IsFacingRight())
            {
                FlipTransform(buffIcon.transform, true);
            } else
            {
                FlipTransform(buffIcon.transform, false);
            }
        }
    }
}
