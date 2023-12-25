using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionClass : SelectClass
{
    //The following are 'properties' in C# that make quick getters and setters for private fields. ac.Target for access
    public EntityClass Target { get; set; }
    public EntityClass Origin { get; set; }

    protected int damage { get; set; }
    protected int block { get; set; }
    protected int speed { get; set; }

    public abstract void ExecuteActionEffect();

    public override void OnMouseDown()
    {
        Debug.Log("Card has been Clicked !!");
        HighlightManager.OnActionClicked(this);
    }

    public int getDamage() {
        return damage;
    }

    public int getBlock()
    {
        return block;
    }

    public int getSpeed()
    {
        return speed;
    }
}
