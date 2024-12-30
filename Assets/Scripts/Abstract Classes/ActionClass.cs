using System;
using Systems.Persistence;
using UnityEngine;

public abstract class ActionClass : SelectClass, IBind<ActionData>
{
    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
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

    public int CostToAddToDeck { get; set; } = 2;

    protected int lowerBound;
    protected int upperBound;

    public int LowerBound => lowerBound;

    public int UpperBound => upperBound;

    protected RolledStats rolledCardStats = new();

    public RolledStats GetRolledStats()
    {
        return rolledCardStats;
    }

    // Struct to Apply Buffs to so as to avoid modifying the cards
    public struct RolledStats
    {
        public int rollFloor;
        public int rollCeiling;
        public int actualRoll;
        //We want to render these one time buffs so we keep track of its name, lower and upper bound buffs to this card.
        public (string, int, int) oneTimeBuffs; //Left int represents lower bound increased by buff. Right int represents the upper bound
    }

    public enum CardState
    {
        NORMAL,
        HOVER,
        CANT_PLAY,
        CLICKED_STATE,
    }

    private CardState cardState = CardState.NORMAL;
    public CardType CardType { get; protected set; }


public int Speed { get; set; }
    public string description;
    public string Description { get { return description; } }
    public string evolutionDescription { get; protected set; }
    public string evolutionCriteria{ get; protected set; }

    public bool IsEvolved = false; // Stores whether the user has decided to select the evolved version of the card or not
    public bool IsFlipped { get; set; } = false; // Should we display the card as flipped?
    [SerializeField]
    private Sprite icon;
    public Sprite cardBack;
    public Sprite evolvedCardBack;
    public CardUI cardUI;

#nullable enable
    [SerializeField] protected ActionData? data;
    protected int CurrentEvolutionProgress
    {
        get { return data?.CurrentProgress ?? 0; }
        set { if (data != null) data.CurrentProgress = Math.Min(value, MaxEvolutionProgress); }
    }
    protected int MaxEvolutionProgress { get; set; }

    public delegate void ActionClassDelegate(ActionClass target);
    public event ActionClassDelegate? TargetChanged;
    public event ActionClassDelegate? TargetChanging;
    public event ActionClassDelegate? CardValuesUpdating;
    public static event ActionClassDelegate? CardClickedEvent;
    public static event ActionClassDelegate? CardRightClickedEvent;
    public static event ActionClassDelegate? CardHighlightedEvent;
    public static event ActionClassDelegate? CardUnhighlightedEvent;
    public delegate void CardStateDelegate(CardState previousState, CardState currentState);
    public static event CardStateDelegate? CardStateChange;


    public virtual void OnQueue() { }
    public virtual void OnRetrieveFromQueue() { }
    public virtual void OnCardStagger() { }
    public virtual void CardIsUnstaggered() { }
    public virtual void ClashWon() => Origin.combatInfo.setDiceColor(Color.green);
    public virtual void ClashTied() => Origin.combatInfo.setDiceColor(Color.white);
    public virtual void ClashLost() => Origin.combatInfo.setDiceColor(Color.red);
    

    public virtual void Awake()
    {
        Initialize();
    }

    public virtual void Start() { }

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

    private void UpdateBuffValue(EntityClass origin)
    {
        UpdateDup();
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
        this.Target.TakeDamage(Origin, rolledCardStats.actualRoll);
    }

    //Only called in a clash
    public virtual void OnDefendClash(ActionClass opposingCard)
    {
        Origin.BlockAnimation(); //Blocked stuff animation here not implemented properly
        opposingCard.ReduceRoll(GetRolledStats().actualRoll);
    }

    public virtual bool IsPlayableByPlayer(out PopupType popupType)
    {
        bool canInsert = BattleQueue.BattleQueueInstance.CanInsertPlayerCard(this);
        popupType = canInsert ? PopupType.None : PopupType.SameSpeed;
        return canInsert;
    }

    public bool IsPlayedByPlayer()
    {
        return Origin.Team == EntityTeam.PlayerTeam;
    }

    public virtual void Initialize()
    {
        UpdateDup();
    }

    public int getRolledDamage()
    {
        return rolledCardStats.actualRoll;
    }

    // Initializes a CardDup struct with the given stats of the Card to 
    // modify further based on buffs
    private void DupInit()
    {
        RolledStats oldDup = rolledCardStats;
        rolledCardStats = new RolledStats();
        rolledCardStats.rollFloor = lowerBound;
        rolledCardStats.rollCeiling = upperBound;
        rolledCardStats.actualRoll = oldDup.actualRoll;
        rolledCardStats.oneTimeBuffs = ("", 0, 0);
    }

    public void ReduceRoll(int byValue)
    {
        rolledCardStats.actualRoll = Mathf.Clamp(rolledCardStats.actualRoll - byValue, 0, rolledCardStats.actualRoll);
    }

    public void IncrementRoll(int byValue)
    {
        rolledCardStats.actualRoll += byValue;
    }

    public void UpdateDup()
    {
        DupInit();
        Origin?.ApplyAllBuffsToCard(ref rolledCardStats);
        UpdateText();
    }

    public virtual void ApplyEffect()
    {
        UpdateDup();
        Origin?.ApplySingleUseEffects(ref rolledCardStats);
        UpdateText();
    }

    // Calculates Actual Damage/Block After Applying Buffs
    public virtual void RollDice()
    {
        rolledCardStats.actualRoll = UnityEngine.Random.Range(rolledCardStats.rollFloor, rolledCardStats.rollCeiling + 1);

        Origin.SetDice(rolledCardStats.actualRoll);
    }

    public Sprite? GetIcon()
    {
        if (icon)
        {
            return icon;
        }
        else
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
    public void OnMouseDown()
    {
        CardClickedEvent?.Invoke(this);
    }

    // To handle right click detection for the Deck Selection scene
    public void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            CardRightClickedEvent?.Invoke(this);
        }
    }

    // Can we evolve this card?
    public bool CanEvolve()
    {
        return CurrentEvolutionProgress >= MaxEvolutionProgress;
    }

    public void ToggleSelected()
    {
        SetCardState(CardState.CLICKED_STATE);
    }

    public void ToggleUnSelected()
    {
        SetCardState(CardState.HOVER);
    }

    public void OnMouseEnter()
    {
        if (PauseMenu.IsPaused) return;
        if (cardState == CardState.NORMAL)
        {
            SetCardState(CardState.HOVER);
        }

        CardHighlightedEvent?.Invoke(this);
    }

    public void OnMouseExit()
    {
        if (PauseMenu.IsPaused) return;
        if (cardState == CardState.HOVER)
        {
            SetCardState(CardState.NORMAL);
        }

        CardUnhighlightedEvent?.Invoke(this);
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
    public void SetCardState(CardState nextState)
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

    public void Bind(ActionData data)
    {
        this.data = data;
        this.data.Id = Id;
    }

    // Returns a description based on whether the card is flipped and evolved or not
    public string GenerateCardDescription()
    {
        if (description != null && !IsFlipped)
        {
            return description;
        }
        else if (evolutionDescription != null && IsFlipped)
        {
            string tempDescription = evolutionDescription;
            if (!CanEvolve())
                tempDescription = "LOCKED: " + evolutionDescription;
            return tempDescription;
        }
        return "";
    }
}

[Serializable]
public class ActionData : ISaveable
{
    public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
    [field: SerializeField] public string ActionClassName { get; set; } = "";
    [field: SerializeField] public int CurrentProgress { get; set; } = 0;
}
