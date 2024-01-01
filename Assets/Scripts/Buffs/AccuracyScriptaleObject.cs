using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccuracyScriptaleObject : StatusEffectScriptableObject
{
    public const string buffName = "Accuracy";

    public override void GainStacks(ref ActionClass.CardDup dup)
    {
        this.buffStacks = dup.totalAccuracy;
        dup.totalAccuracy = this.buffStacks;
    }
}
