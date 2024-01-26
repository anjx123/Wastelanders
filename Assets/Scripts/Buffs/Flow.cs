using UnityEngine;


public class Focus : StatusEffect
{
    public const string buffName = "Flow";

    
    public override void ApplyStacks(ref ActionClass.CardDup dup)
    {
        dup.rollFloor += this.buffStacks;
        dup.rollCeiling += this.buffStacks;
    }

    public override Sprite GetIcon()
    {
        Sprite buffSprite = Resources.Load<Sprite>("StatusIcon/" + buffName);
        if (!buffSprite)
        {
            Debug.LogWarning("Buff Sprite is missing");
        }
        return buffSprite;
    }
}
