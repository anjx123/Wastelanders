using UnityEngine;


public class Accuracy : StatusEffect
{
    public const string buffName = "Accuracy";

    public override void GainStacks(ref ActionClass.CardDup dup)
    {
        this.buffStacks += dup.totalAccuracy;
    }

    public override void ApplyStacks(ref ActionClass.CardDup dup)
    {
        dup.rollFloor = Mathf.Clamp(dup.rollFloor + this.buffStacks, 0, dup.rollCeiling);
    }
}
