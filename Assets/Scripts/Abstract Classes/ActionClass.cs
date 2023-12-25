using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionClass : SelectClass
{
    //The following are 'properties' in C# that make quick getters and setters for private fields. ac.Target for access
    public EntityClass Target { get; set; }
    public EntityClass Origin { get; set; }

    public int Damage { get; protected set; }
    public int Block { get; protected set; }
    public int Speed { get; protected set; }

    public abstract void ExecuteActionEffect();

    public override void OnMouseDown()
    {
        HighlightManager.OnActionClicked(this);
    }
    public virtual void OnHit()
    {
        this.Target.TakeDamage(Damage);
    }
}
