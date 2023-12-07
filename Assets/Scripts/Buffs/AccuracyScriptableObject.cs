using UnityEngine;

[CreateAssetMenu(fileName = "AccuracyScriptableObject", menuName = "ScriptableObject/Accuracy")]
public class AccuracyScriptableObject : StatusEffectScriptableObject
{
    public const string buffName = "Accuracy";

    public override void GainStacks(ref ActionClass.CardDup dup)
    {
        this.buffStacks = dup.totalAccuracy;
        dup.rollFloor = Mathf.Clamp(dup.rollFloor + this.buffStacks, 0, dup.rollCeiling);
    }
}
