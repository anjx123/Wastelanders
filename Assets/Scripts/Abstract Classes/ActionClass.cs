using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using TMPro;

public abstract class ActionClass : SelectClass
{

    //The following are 'properties' in C# that make quick getters and setters for private fields. ac.Target for access
    private EntityClass target;
    public EntityClass Target
    {
        get { return target; }
        set
        {
            targetChanged?.Invoke(this);
            target = value;
        }
    }
    private EntityClass origin;
    public delegate void ActionClassDelegate(ActionClass target);
    public event ActionClassDelegate targetChanged;
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
    protected ActionClass? activeDupCardInstance;
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
    public enum CardState
    {
        NORMAL,
        HOVER,
        CANT_PLAY,
        CLICKED_STATE,
    }

    private CardState cardState = CardState.NORMAL;
    public int Speed { get; protected set; }
    public string description;

    [SerializeField] string titleName;

    public Sprite icon;

    protected Sprite fullCard; // used for displaying combat info
    public GameObject fullCardObjectPrefab;

    [SerializeField] private CardUI cardUI;

    public CardType CardType { get; protected set; }

    protected Vector3 OriginalPosition;

#nullable enable
    public delegate void CardStateDelegate(CardState cardState);
    public delegate void CardEventDelegate(ActionClass card);
    public static event CardEventDelegate? cardClickedEvent;
    public static event CardEventDelegate? cardHighlightedEvent;
    public static event CardStateDelegate? CardSelectedEvent;

    public virtual void ExecuteActionEffect()
    {

    }

    public virtual void Awake()
    {
        Initialize();
    }

    public virtual void Start()
    {

    }
    //@Author: Anrui
    //Called when this card hits the enemy, runs any on hit buffs or effects given.
    //Note: that OnHit implies CardIsUnstaggered, thus it calls it. Please be **very careful** about the timing that CardIsUnstaggered is called. 
    // This also implies that OnHit is highly unlikely to be overriden in ANY derived class: tge emphasis should almost entirely be on CardIsUnstaggered
    // (excepting few cases like StackSmash that has a unique Animation)
    public virtual void OnHit()
    {
        Vector3 diffInLocation = Target.myTransform.position - Origin.myTransform.position;
        Origin.UpdateFacing(diffInLocation, null);
        CardIsUnstaggered();
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

    public Sprite? GetIcon()
    {
        if (icon)
        {
            return icon;
        } else
        {
            Debug.LogWarning("ActionClass icon is Missing for " + name);
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
    public override void OnMouseDown()
    {
        HighlightManager.OnActionClicked(this);
        cardClickedEvent?.Invoke(this);
    }

    public void ToggleSelected()
    {
        SetCardState(CardState.CLICKED_STATE);
    }

    public void ToggleUnSelected()
    {
        SetCardState(CardState.HOVER);
    }

    public override void OnMouseEnter()
    {
        if (cardState == CardState.NORMAL)
        {
            SetCardState(CardState.HOVER);
        }

        if (description != null)
        {
            PopUpNotificationManager.Instance.DisplayText(description);
        }
        cardHighlightedEvent?.Invoke(this);
    }

    public override void OnMouseExit()
    {
        if (cardState == CardState.HOVER)
        {
            SetCardState(CardState.NORMAL);
        }

        PopUpNotificationManager.Instance.RemoveDescription();
    }

    public void SetCanPlay(bool canPlay)
    {
        if (canPlay)
        {
            SetCardState(CardState.NORMAL);
        }
        else
        {
            SetCardState(CardState.CANT_PLAY);
        }
    } 

    public void ForceNormalState()
    {
        if (cardState == CardState.CLICKED_STATE)
        {
            SetCardState(CardState.HOVER);
            SetCardState(CardState.NORMAL);
        }
    }
    private void SetCardState(CardState nextState)
    {
        CardState previousState = cardState;
        
        //Transition Diagram
        // CANT_PLAY <-> NORMAL <-> HOVER <-> CLICKED_STATE

        //Bounces all the bad state transitions
        if ((previousState == CardState.CANT_PLAY && nextState != CardState.NORMAL) ||
            (previousState == CardState.NORMAL && (nextState != CardState.CANT_PLAY && nextState != CardState.HOVER)) || 
            (previousState == CardState.HOVER && (nextState != CardState.NORMAL && nextState != CardState.CLICKED_STATE)) ||
            (previousState == CardState.CLICKED_STATE && (nextState != CardState.HOVER)))
        {
            Debug.Log("Bouncing my state where previous: " + previousState + " and current: " + nextState);
            return;
        }

        // Determine the color and position changes based on the current and new states
        Color newColor = Color.white;
        Vector3 positionChange = Vector3.zero;

        switch (nextState)
        {
            case CardState.NORMAL:
                newColor = Color.white;
                if (previousState == CardState.HOVER)
                {
                    positionChange = new Vector3(-0.04f, -0.4f, 0);
                }
                break;
            case CardState.HOVER:
                newColor = new Color(0.6f, 0.6f, 0.6f, 1);
                if (previousState == CardState.NORMAL)
                {
                    positionChange = new Vector3(0.04f, 0.4f, 0);
                }
                if (previousState == CardState.CLICKED_STATE)
                {
                    CardSelectedEvent?.Invoke(CardState.HOVER);
                }
                break;
            case CardState.CANT_PLAY:
                newColor = new Color(1f, 200f / 255f, 200f / 255f, 1);
                if (cardState == CardState.HOVER)
                {
                    positionChange = new Vector3(-0.04f, -0.4f, 0); //This state cant happen?
                }
                break;
            case CardState.CLICKED_STATE:
                newColor = new Color(0.5f, 0.5f, 0.5f, 1);
                if (previousState == CardState.HOVER)
                {
                    CardSelectedEvent?.Invoke(CardState.CLICKED_STATE);
                }
                break;

        }

        // Apply the new state
        cardState = nextState;

        // Apply the color and position changes
        GetComponent<SpriteRenderer>().color = newColor;
        transform.position += positionChange;

        // Apply the same color change to child SpriteRenderers
        SpriteRenderer[] childSpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer spriteRenderer in childSpriteRenderers)
        {
            spriteRenderer.color = newColor;
        }
    }





    //Will activate the checkmark on card UI for indication that it is in the player's deck
    public void SetSelectedForDeck(bool isSelectedForDeck)
    {
        cardUI.SetSelectedForDeck(isSelectedForDeck);
    }
}
