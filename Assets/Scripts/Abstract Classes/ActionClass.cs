using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionClass : SelectClass
{
    //The following are 'properties' in C# that make quick getters and setters for private fields. ac.Target for access
    public EntityClass Target { get; set; }
    public EntityClass Origin { get; set; }

    protected int lowerBound;
    protected int upperBound;
    protected int focus;
    protected int accuracy;

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
        public int totalFocus;
        public int totalAccuracy;
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

    public override void OnMouseDown()
    {
        HighlightManager.OnActionClicked(this);
    }
    public virtual void OnHit()
    {
        float percentageDone = 1; //Testing different powered knockbacks
        if (Target.Health != 0)
        {
            percentageDone = Mathf.Clamp(duplicateCard.actualRoll / (float)Target.Health, 0f, 1f);
        }
        this.Target.TakeDamage(duplicateCard.actualRoll);
        CardComparator.Instance.StartStagger(Origin, Target, percentageDone);
    }

    public bool IsPlayedByPlayer()
    {
        return Origin.GetType().IsSubclassOf(typeof(PlayerClass));
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
        duplicateCard.totalFocus = focus;
        duplicateCard.totalAccuracy = accuracy;
    }

    public virtual void ApplyEffect()
    {
        DupInit();
    }

    // Calculates Actual Damage/Block After Applying Buffs
    public virtual void RollDice()
    {
        duplicateCard.actualRoll = Random.Range(duplicateCard.rollFloor, duplicateCard.rollCeiling + 1);
        
        Origin.SetDice(Damage); 
    }

    public Sprite GetIcon()
    {
        if (icon)
        {
            return icon;
        } else
        {
            Debug.LogWarning("Icon is Missing");
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
