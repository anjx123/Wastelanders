using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionClass : SelectClass
{
    //The following are 'properties' in C# that make quick getters and setters for private fields. ac.Target for access
    public EntityClass Target { get; set; }
    public EntityClass Origin { get; set; }

    protected int damage;
    public int Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    
    protected int block;
    public int Block
    {
        get { return block; }
        set { block = value; }
    }
    protected int speed;

    public CardType CardType { get; set; }

    public abstract void ExecuteActionEffect();

    public virtual void OnHit()
    {
        this.Target.TakeDamage(Damage);
    }


    public override void OnMouseDown()
    {
        Debug.Log("Card has been Clicked !!");
        HighlightManager.OnActionClicked(this);
    }

    public int getDamage() {
        return Damage;
    }

    public void setDamage(int damage)
    {
        Damage = damage;
    }


}
