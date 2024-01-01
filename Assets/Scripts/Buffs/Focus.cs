using UnityEngine;


public class Focus : StatusEffect
{
    public const string buffName = "Focus";

    public override void GainStacks(ref ActionClass.CardDup dup)
    {
        this.buffStacks += dup.totalFocus;
    }

    public override void ApplyStacks(ref ActionClass.CardDup dup)
    {
        dup.rollFloor += this.buffStacks;
        dup.rollCeiling += this.buffStacks;
    }
}
