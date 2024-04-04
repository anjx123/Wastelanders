using Unity.VisualScripting;
using UnityEngine;


public class Flow : StatusEffect
{
    public const string buffName = "Flow";

    
    public override void ApplySingleUseEffects(ref ActionClass.CardDup dup)
    {
        dup.rollFloor += this.buffStacks;
        dup.rollCeiling += this.buffStacks;
        dup.oneTimeBuffs = (buffName, buffStacks, buffStacks);
        ClearBuff();
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
