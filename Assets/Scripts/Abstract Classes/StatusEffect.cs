using UnityEngine;

public abstract class StatusEffect
{
    public int buffStacks = 0;
    // public static string fieldName; possessed by all non-abstract children

    // increments buffStacks by amount in the card
    public abstract void GainStacks(ref ActionClass.CardDup dup);

    // adds buffStacks to the struct card limits
    public abstract void ApplyStacks(ref ActionClass.CardDup dup);

    // clears buffStacks
    public void ClearBuff() { buffStacks = 0; }


}