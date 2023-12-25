using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionClass : SelectClass
{
    //The following are 'properties' in C# that make quick getters and setters for private fields. ac.Target for access
    public EntityClass Target { get; set; }
    public EntityClass Origin { get; set; }

    public int Damage { get; set; }
    public int Block { get; set; }
    public int Speed { get; set; }
    public CardType CardType { get; set; }

    public abstract void ExecuteActionEffect();

    public override void OnMouseDown()
    {
        Debug.Log("Card has been Clicked !!");
        HighlightManager.OnActionClicked(this);
    }
    public virtual void OnHit()
    {
        this.Target.TakeDamage(Damage);
    }
}
