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
    protected bool cardType; // 1 (true) indicates damage, 0 (false) indicates block
    protected int damage;
    protected int speed;
    protected int focus;
    protected int accuracy;

    protected CardDup duplicateCard;

    public CardDup GetCard()
    {
        return duplicateCard;
    }

    public struct CardDup
    {
        public int rollFloor;
        public int rollCeiling;
        public int totalFocus;
        public int totalAccuracy;
        public int actualRoll;
    }

    public abstract void ExecuteActionEffect();

    public override void OnMouseDown()
    {
        Debug.Log("Card has been Clicked !!");
        HighlightManager.OnActionClicked(this);
    }

    public int getDamage() {
        return damage;
    }

    public int getRolledDamage()
    {
        return duplicateCard.actualRoll;
    }

    protected void DupInit()
    {
        duplicateCard = new CardDup();
        duplicateCard.rollFloor = lowerBound;
        duplicateCard.rollCeiling = upperBound;
        duplicateCard.totalFocus = focus;
        duplicateCard.totalAccuracy = accuracy;

        Debug.Log("Duplicate Card Initialized" + duplicateCard.rollFloor + duplicateCard.rollCeiling + duplicateCard.totalFocus + duplicateCard.totalAccuracy);
    }

    public abstract void ApplyEffect();

    public void RollDice()
    {
        duplicateCard.actualRoll = Random.Range(duplicateCard.rollFloor, duplicateCard.rollCeiling + 1);
        Debug.Log("Ref" + duplicateCard.rollFloor + duplicateCard.rollCeiling + duplicateCard.actualRoll);

    }
}
