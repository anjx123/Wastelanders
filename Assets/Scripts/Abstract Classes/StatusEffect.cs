using UnityEngine;
using UnityEngine.EventSystems;

public abstract class StatusEffect
{
    protected int buffStacks = 0;
    public int Stacks { get { return buffStacks; } }
    public delegate void StatusEffectDelegate();
    public delegate void StatusEffectModifyValueDelegate(ref int integer);
    // public static string fieldName; possessed by all non-abstract children

    public int GetStacks()
    {
        return this.buffStacks;
    }

    // increments buffStacks by the amount given    
    public virtual void GainStacks(int stacks)
    {
        this.buffStacks += stacks;
    }

    // lowers buffStacks by amount specified
    public void LoseStacks(int amount)
    {
        this.buffStacks = Mathf.Clamp(this.buffStacks - amount, 0, this.buffStacks);
    }

    public StatusEffectModifyValueDelegate OnEntityHitHandler { get; set; } = delegate(ref int damage){ };

    //Should swap these out for Handlers
    public virtual void NewRound() { }

    // adds buffStacks to the struct card limits
    public virtual void ApplyStacks(ref ActionClass.CardDup dup)
    {

    }

    // adds buffStacks to the struct card limits
    public virtual void ApplySingleUseEffects(ref ActionClass.CardDup dup)
    {

    }


    // clears buffStacks
    public void ClearBuff() { buffStacks = 0; }

    //Whenever a new Buff is created, make sure to add its Icon to Resources folder
    public abstract Sprite GetIcon();

}