﻿using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Resonate : StatusEffect
{
    public const string buffName = "Resonate";

    public override void ApplyStacks(ref ActionClass.RolledStats dup)
    {
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