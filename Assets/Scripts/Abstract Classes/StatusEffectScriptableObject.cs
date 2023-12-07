using UnityEngine;

public abstract class StatusEffectScriptableObject : ScriptableObject
{
    public int buffStacks = 0;
    // public static string fieldName; possessed by all non-abstract children

    // increments buffStacks by amount in the card, then returns new amount
    public abstract void GainStacks(ref ActionClass.CardDup dup);

    // clears buffStacks
    public void ClearBuff() { buffStacks = 0; }


}