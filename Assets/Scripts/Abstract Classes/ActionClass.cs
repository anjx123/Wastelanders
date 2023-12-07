using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionClass : SelectClass
{
    //The following are 'properties' in C# that make quick getters and setters for private fields. ac.Target for access
    public EntityClass Target { get; set; }
    public EntityClass Origin { get; set; }

    protected int damage;
    protected int block;
    protected int speed;
    protected int cardWielder = 1; //1: player's card, 0: enemy's card. 1 for now.

    public abstract void ExecuteActionEffect();

    public override void OnMouseDown()
    {
        Debug.Log("Card has been Clicked !!");
        HighlightManager.OnActionClicked(this);
    }

    public int getDamage() {
        return damage;
    }

    public int getSpeed() {
        return speed;
    }

    public int getCardWielder() {
        return cardWielder;
    }


}
