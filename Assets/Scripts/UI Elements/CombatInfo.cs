using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatInfo : MonoBehaviour
{
    [SerializeField] private GameObject combatCardIconPrefab;
    [SerializeField] private GameObject cardIconRendering;
    private List<ActionClass> combatCards = new();
    private ActionClass activeActionClass;
    public HorizontalLayoutGroup buffList;
    public Animator diceAnimator;
    public GameObject diceRollSprite;
    public List<Sprite> loadedSprites = new();
    public GameObject diceRollText;
    public GameObject buffIconPrefab;
    public HealthBar healthBar;
    public GameObject crosshair;

    private float ROTATION_SPEED = 30f;

    public Canvas buffListCanvas;

    public void Awake()
    {
        buffListCanvas = buffList.gameObject.GetComponent<Canvas>();
    }

    public void Start()
    {
        diceRollText.GetComponent<MeshRenderer>().sortingOrder = diceRollSprite.GetComponent<SpriteRenderer>().sortingOrder + 1;
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
    public void setDiceColor(Color color)
    {
        diceRollText.GetComponent<TMP_Text>().color = color;
    }

    public void ActivateCombatSprite(ActionClass actionClass)
    {
        diceAnimator.enabled = true;
        diceRollText.GetComponent<TextMeshPro>().text = null;
        if (!combatCards.Contains(actionClass))
        {
            combatCards.Add(actionClass);
            RenderCombatIcons();
        }
        activeActionClass = actionClass;
    }

    public void AddCombatSprite(ActionClass actionClass)
    {
        combatCards.Add(actionClass);
        RenderCombatIcons();
    }

    private void RenderCombatIcons()
    {
        int num = combatCards.Count;
        float iconHeight = combatCardIconPrefab.GetComponent<SpriteRenderer>().bounds.size.y;
        float totalHeight = num * iconHeight;
        float startY = totalHeight / 2 - iconHeight / 2;

        UnrenderCombatIcons();
        for (int i = 0; i < num; i++)
        {
            GameObject combatIcon = Instantiate(combatCardIconPrefab);
            combatIcon.transform.SetParent(cardIconRendering.transform);
            combatIcon.transform.localScale = Vector3.one;
            combatIcon.transform.localPosition = new Vector3(0, startY - i * iconHeight, 0);
            combatIcon.GetComponent<CombatCardUI>().SetActionClass(combatCards[num - i - 1]); //Reverse the order of rendering 
            combatIcon.GetComponent<CombatCardUI>().DeEmphasize();
        }
    }
    private void UnrenderCombatIcons()
    {
        foreach (Transform child in cardIconRendering.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void EmphasizeCombatIcon()
    {
        bool onlyHighlightOne = true;
        foreach (Transform child in cardIconRendering.transform)
        {
            CombatCardUI combatUI = child.GetComponent<CombatCardUI>();
            if (child.GetComponent<CombatCardUI>().ActionClass == activeActionClass && onlyHighlightOne)
            {
                combatUI.Emphasize();
                onlyHighlightOne = false;
            }
            else
            {
                combatUI.DeEmphasize();
            }
        }
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

    public void EnableHealthBar()
    {
        healthBar.gameObject.SetActive(true);
    }

    public void DisableHealthBar()
    {
        healthBar.gameObject.SetActive(false);
    }

    public void DeactivateCombatSprite(ActionClass actionClass)
    {
        combatCards.Remove(actionClass);
        RenderCombatIcons();
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
        EmphasizeCombatIcon();
        diceRollSprite.GetComponent<SpriteRenderer>().sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER + 1;
        buffListCanvas.sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER + 1;
        diceRollText.GetComponent<MeshRenderer>().sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER + 1;
        healthBar.Emphasize();
    }
    public void DeEmphasize()
    {
        foreach (Transform child in cardIconRendering.transform)
        {
            child.GetComponent<CombatCardUI>().DeEmphasize();
        }
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

    private bool IsFacingRight()
    {
        return this.gameObject.transform.lossyScale.x > 0;
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
            }
            else
            {
                FlipTransform(buffIcon.transform, false);
            }
        }
    }
}
