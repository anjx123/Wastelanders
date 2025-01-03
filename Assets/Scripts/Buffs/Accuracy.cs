using UnityEngine;


public class Accuracy : StatusEffect
{
    public const string buffName = "Accuracy";

    public Accuracy()
    {
        OnEntityHitHandler = OnEntityHit;
    }
    
    public override void ApplyStacks(ref ActionClass.RolledStats dup)
    {
        dup.rollFloor = Mathf.Clamp(dup.rollFloor + this.buffStacks, 0, dup.rollCeiling);
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

    //Getting hit with accuracy halves it 
    void OnEntityHit(ref int damage)
    {
        if (damage > 0)
        {
            buffStacks = Stacks / 2;
        }
    }
}
