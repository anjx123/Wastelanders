using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using TMPro;

public abstract class ActionClass : SelectClass
{
    //The following are 'properties' in C# that make quick getters and setters for private fields. ac.Target for access
    public EntityClass Target { get; set; }
    private EntityClass origin;

    public EntityClass Origin
    {
        get { return origin; }
        set
        {
            origin = value;
            UpdateDup();
        }
    }

    protected int lowerBound;
    protected int upperBound;

    #nullable enable
    [SerializeField] protected GameObject? duplicateCardInstance; // set in editor for now
    protected ActionClass activeDupCardInstance;
    protected bool proto = true;
    #nullable disable 

    protected CardDup duplicateCard;

    public CardDup GetCard()
    {
        return duplicateCard;
    }

    // Struct to Apply Buffs to so as to avoid modifying the cards
    public struct CardDup
    {
        public int rollFloor;
        public int rollCeiling;
        public int actualRoll;
    }
    public int Speed { get; protected set; }
    protected string description;

    [SerializeField] string titleName;

    public Sprite icon;

    protected Sprite fullCard; // used for displaying combat info
    public GameObject fullCardObjectPrefab;

    [SerializeField] private CardUI cardUI;

    public CardType CardType { get; protected set; }

    protected Vector3 OriginalPosition;

    protected bool EnqueueMoveDown = false;

    public abstract void ExecuteActionEffect();

    public virtual void Awake()
    {
        Initialize();
    }

    public virtual void Start()
    {

    }

    public override void OnMouseDown()
    {
        HighlightManager.OnActionClicked(this);
    }
    //@Author: Anrui
    //Called when this card hits the enemy, runs any on hit buffs or effects given.
    //Note: that OnHit implies CardIsUnstaggered, thus it calls it. Please be **very careful** about the timing that CardIsUnstaggered is called. 
    // This also implies that OnHit is highly unlikely to be overriden in ANY derived class: tge emphasis should almost entirely be on CardIsUnstaggered
    // (excepting few cases like StackSmash that has a unique Animation)
    public virtual void OnHit()
    {
        CardIsUnstaggered();
        Vector3 diffInLocation = Target.myTransform.position - Origin.myTransform.position;
        Origin.UpdateFacing(diffInLocation, null);
        this.Target.TakeDamage(Origin, duplicateCard.actualRoll);
    }

    public virtual void CardIsUnstaggered()
    {

    }

    public bool IsPlayedByPlayer()
    {
        return Origin.GetType().IsSubclassOf(typeof(PlayerClass));
    }

    public virtual void Initialize()
    {
        UpdateDup();
    }

    public int getRolledDamage()
    {
        return duplicateCard.actualRoll;
    }

    // Initializes a CardDup struct with the given stats of the Card to 
    // modify further based on buffs
    private void DupInit()
    {
        duplicateCard = new CardDup();
        duplicateCard.rollFloor = lowerBound;
        duplicateCard.rollCeiling = upperBound;
    }

    public void ReduceRoll(int byValue)
    {
        duplicateCard.actualRoll = Mathf.Clamp(duplicateCard.actualRoll - byValue, 0, duplicateCard.actualRoll);
    }

    public void IncrementRoll(int byValue)
    {
        duplicateCard.actualRoll += byValue;
    }

    public void UpdateDup()
    {
        DupInit();
        Origin?.ApplyAllBuffsToCard(ref duplicateCard);
        UpdateText();
    }

    public virtual void ApplyEffect()
    {
        UpdateDup();
    }

    // Calculates Actual Damage/Block After Applying Buffs
    public virtual void RollDice()
    {
        duplicateCard.actualRoll = Random.Range(duplicateCard.rollFloor, duplicateCard.rollCeiling + 1);
        
        Origin.SetDice(duplicateCard.actualRoll); 
    }

    public Sprite GetIcon()
    {
        if (icon)
        {
            return icon;
        } else
        {
            Debug.LogWarning("ActionClass icon is Missing");
            return null;
        }
    }

    private void UpdateText()
    {
        Transform textContainerTransform = transform.Find("TextCanvas");
        if (textContainerTransform == null)
        {
            return;
        }
        GameObject textContainer = textContainerTransform.gameObject;
        TextMeshProUGUI NameText = textContainer.transform.Find("NameText").gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI lowerBoundText = textContainer.transform.Find("LowerBoundText").gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI upperBoundText = textContainer.transform.Find("UpperBoundText").gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI SpeedText = textContainer.transform.Find("SpeedText").gameObject.GetComponent<TextMeshProUGUI>();

        // Set the text first
        NameText.text = titleName;
        lowerBoundText.text = duplicateCard.rollFloor.ToString();
        upperBoundText.text = duplicateCard.rollCeiling.ToString();
        SpeedText.text = Speed.ToString();

        // Now update colors
        if (duplicateCard.rollFloor > lowerBound)
        {
            lowerBoundText.color = Color.green;
        }

        if (duplicateCard.rollCeiling > upperBound)
        {
            upperBoundText.color = Color.green;
        }

        if (duplicateCard.rollFloor < lowerBound)
        {
            lowerBoundText.color = Color.red;
        }

        if (duplicateCard.rollCeiling < upperBound)
        {
            upperBoundText.color = Color.red;
        }

        if (duplicateCard.rollFloor == lowerBound)
        {
            lowerBoundText.color = Color.black;
        }

        if (duplicateCard.rollCeiling == upperBound)
        {
            upperBoundText.color = Color.black;
        }

    }


    public override void OnMouseEnter()
    {
        if (!isOutlined)
        {
            GetComponent<SpriteRenderer>().color = new Color(0.6f, 0.6f, 0.6f, 1);

            // Get the SpriteRenderers of the child objects
            SpriteRenderer[] childSpriteRenderers = GetComponentsInChildren<SpriteRenderer>();

            // Now we can access each SpriteRenderer
            foreach (SpriteRenderer spriteRenderer in childSpriteRenderers)
            {
                spriteRenderer.color = new Color(0.6f, 0.6f, 0.6f, 1);
            }

            transform.position += new Vector3((float)0.04, (float)0.4, 0);
        }
        else
        {
            EnqueueMoveDown = false;
        }

        if (description != null)
        {
            PopUpNotificationManager.Instance.DisplayText(description);
        }
    }

    public override void OnMouseExit()
    {
        if (!isOutlined)
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1);

            SpriteRenderer[] childSpriteRenderers = GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer spriteRenderer in childSpriteRenderers)
            {
                spriteRenderer.color = new Color(1f, 1f, 1f, 1);
            }

            transform.position -= new Vector3((float)0.04, (float)0.4, 0);
        }
        else
        {
            EnqueueMoveDown = true;
        }

        PopUpNotificationManager.Instance.RemoveDescription();
    }

    public override void Highlight()
    {
        isOutlined = true;
        GetComponent<SpriteRenderer>().color = new Color(0.6f, 0.6f, 0.6f, 1);
        SpriteRenderer[] childSpriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer spriteRenderer in childSpriteRenderers)
        {
            spriteRenderer.color = new Color(0.6f, 0.6f, 0.6f, 1);
        }
        transform.rotation = Quaternion.Euler(0, 0, 0);
        CombatManager.Instance.CrosshairAllEnemies();
    }

    public override void DeHighlight()
    {
        isOutlined = false;
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1);
        SpriteRenderer[] childSpriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer spriteRenderer in childSpriteRenderers)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 1);
        }
        transform.rotation = Quaternion.Euler(0, 0, -5);
        if (EnqueueMoveDown)
        {
            transform.position -= new Vector3((float)0.04, (float)0.4, 0);
            EnqueueMoveDown = false;
        }
        CombatManager.Instance.UncrosshairAllEnemies();
    }


    //Will activate the checkmark on card UI for indication that it is in the player's deck
    public void SetSelectedForDeck(bool isSelectedForDeck)
    {
        cardUI.SetSelectedForDeck(isSelectedForDeck);
    }
}
