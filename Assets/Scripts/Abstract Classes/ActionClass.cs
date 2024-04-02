using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public abstract class ActionClass : SelectClass
{

    //The following are 'properties' in C# that make quick getters and setters for private fields. ac.Target for access
    private EntityClass target;
    public EntityClass Target
    {
        get { return target; }
        set
        {
            TargetChanging?.Invoke(this);
            target = value;
            TargetChanged?.Invoke(this);
        }
    }
    private EntityClass origin;
    public delegate void ActionClassDelegate(ActionClass target);
    public event ActionClassDelegate TargetChanged;
    public event ActionClassDelegate TargetChanging;
    public EntityClass Origin
    {
        get { return origin; }
        set
        {
            if (origin != null)
            {
                origin.BuffsUpdatedEvent -= UpdateBuffValue;
            }
            origin = value;
            if (origin != null)
            {
                origin.BuffsUpdatedEvent += UpdateBuffValue;
            }
            UpdateDup();
        }
    }

    private void UpdateBuffValue(EntityClass origin)
    {
        UpdateDup();
    }

    public int CostToAddToDeck { get; set; } = 2;

    protected int lowerBound;
    protected int upperBound;

    public int LowerBound { get { return lowerBound; }}
    public int UpperBound { get { return upperBound; }}

    protected CardDup duplicateCard = new CardDup();

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
    public int Speed { get; set; }
    protected string description;
    public string Description {  get { return description; }}
    [SerializeField] private Sprite icon;
    public Sprite cardBack;
    [SerializeField] private CardUI cardUI;

    public CardType CardType { get; protected set; }

    protected Vector3 OriginalPosition;

#nullable enable
    public delegate void CardStateDelegate(CardState previousState, CardState currentState);
    public delegate void CardEventDelegate(ActionClass card);
    public static event CardEventDelegate? CardClickedEvent;
    public static event CardEventDelegate? CardHighlightedEvent;
    public static event CardStateDelegate? CardStateChange;

    public event CardEventDelegate? CardValuesUpdating;

    public virtual void OnCardStagger()
    {

    }

    public virtual void Awake()
    {
        Initialize();
    }

    public virtual void Start()
    {
        
    }

    private void OnEnable()
    {
        PauseMenu.onPauseMenuActivate += OnMouseExit;
    }

    private void OnDisable()
    {
        if (origin != null)
        {
            origin.BuffsUpdatedEvent -= UpdateBuffValue;
        }
        PauseMenu.onPauseMenuActivate -= OnMouseExit;
    }

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
        CardDup oldDup = duplicateCard;
        duplicateCard = new CardDup();   
        duplicateCard.rollFloor = lowerBound;
        duplicateCard.rollCeiling = upperBound;
        duplicateCard.actualRoll = oldDup.actualRoll;
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
        Origin?.ApplySingleUseEffects(ref duplicateCard);
        UpdateText();
    }

    // Calculates Actual Damage/Block After Applying Buffs
    public virtual void RollDice()
    {
        duplicateCard.actualRoll = UnityEngine.Random.Range(duplicateCard.rollFloor, duplicateCard.rollCeiling + 1);
        
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
        cardUI?.RenderCard(this);
        CardValuesUpdating?.Invoke(this);
    }
    public override void OnMouseDown()
    {
        CardClickedEvent?.Invoke(this);
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
        if (PauseMenu.IsPaused) return;
        if (cardState == CardState.NORMAL)
        {
            SetCardState(CardState.HOVER);
        }

        if (description != null)
        {
            PopUpNotificationManager.Instance.DisplayText(description);
        }
        CardHighlightedEvent?.Invoke(this);
    }

    public override void OnMouseExit()
    {
        if (PauseMenu.IsPaused) return;
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
                    ExtendBoxCollider(gameObject.GetComponent<BoxCollider>(), -1.6f);
                }
                break;
            case CardState.HOVER:
                newColor = new Color(0.6f, 0.6f, 0.6f, 1);
                if (previousState == CardState.NORMAL)
                {
                    positionChange = new Vector3(0.04f, 0.4f, 0);
                    ExtendBoxCollider(gameObject.GetComponent<BoxCollider>(), 1.6f);
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
        CardStateChange?.Invoke(previousState, nextState);
    }

    //extends the boxcollider downwards
    void ExtendBoxCollider(BoxCollider boxCollider, float extendAmount)
    {
        // Store the original size and center
        Vector3 originalSize = boxCollider.size;
        Vector3 originalCenter = boxCollider.center;

        // Extend the size of the BoxCollider along the y-axis
        boxCollider.size = new Vector3(originalSize.x, originalSize.y + extendAmount, originalSize.z);

        // Adjust the center of the BoxCollider to keep the bottom end at the same place
        boxCollider.center = new Vector3(originalCenter.x, originalCenter.y - extendAmount / 2, originalCenter.z);
    }





    //Will activate the checkmark on card UI for indication that it is in the player's deck
    public void SetSelectedForDeck(bool isSelectedForDeck)
    {
        cardUI.SetSelectedForDeck(isSelectedForDeck);
    }

    public void SetRenderCost(bool renderCost)
    {
        cardUI.shouldRenderCost = renderCost;
    }
}
