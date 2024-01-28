using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

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
    public int Damage { get; protected set; }
    public int Block { get; protected set; }
    public int Speed { get; protected set; }

    public Sprite icon;

    public CardType CardType { get; protected set; }

    protected Vector3 OriginalPosition;

    protected bool EnqueueMoveDown = false;

    public abstract void ExecuteActionEffect();

    public virtual void Awake()
    {
        Initialize();
    }
    public override void OnMouseDown()
    {
        HighlightManager.OnActionClicked(this);
    }
    //Called when this card hits the enemy, runs any on hit buffs or effects given.
    public virtual void OnHit()
    {
        Vector3 diffInLocation = Target.myTransform.position - Origin.myTransform.position;
        Origin.UpdateFacing(diffInLocation, null);
        this.Target.TakeDamage(Origin, duplicateCard.actualRoll);
    }

    public bool IsPlayedByPlayer()
    {
        return Origin.GetType().IsSubclassOf(typeof(PlayerClass));
    }

    public virtual void Initialize()
    {

    }

    public int getRolledDamage()
    {
        return duplicateCard.actualRoll;
    }

    // Initializes a CardDup struct with the given stats of the Card to 
    // modify further based on buffs
    protected void DupInit()
    {
        duplicateCard = new CardDup();
        duplicateCard.rollFloor = lowerBound;
        duplicateCard.rollCeiling = upperBound;
    }

    private void UpdateDup()
    {
        DupInit();
        Origin.ApplyAllBuffsToCard(ref duplicateCard);
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


    public override void OnMouseEnter()
    {
        if (!isOutlined)
        {
            GetComponent<SpriteRenderer>().color = new Color(0.6f, 0.6f, 0.6f, 1);
            transform.position += new Vector3((float)0.04, (float)0.4, 0);
        }
        else
        {
            EnqueueMoveDown = false;
        }
    }

    public override void OnMouseExit()
    {
        if (!isOutlined)
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1);
            transform.position -= new Vector3((float)0.04, (float)0.4, 0);
        }
        else
        {
            EnqueueMoveDown = true;
        }
    }

    public override void Highlight()
    {
        isOutlined = true;
        GetComponent<SpriteRenderer>().color = new Color(0.6f, 0.6f, 0.6f, 1);
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public override void DeHighlight()
    {
        isOutlined = false;
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1);
        transform.rotation = Quaternion.Euler(0, 0, -5);
        if (EnqueueMoveDown)
        {
            transform.position -= new Vector3((float)0.04, (float)0.4, 0);
            EnqueueMoveDown = false;
        }
    }
}
