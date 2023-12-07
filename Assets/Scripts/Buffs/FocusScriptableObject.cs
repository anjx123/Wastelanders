using UnityEngine;


[CreateAssetMenu(fileName = "BuffScriptableObject", menuName = "ScriptableObject/Buff")]
public class FocusScriptableObject : StatusEffectScriptableObject
{
    public const string buffName = "Focus";

    public override void GainStacks(ref ActionClass.CardDup dup)
    {
        this.buffStacks += dup.totalFocus;
        dup.rollFloor += this.buffStacks;
        dup.rollCeiling += this.buffStacks;
    }
}
