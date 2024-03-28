using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

using static StatusEffect;

public class Mutilate : AxeCards
{
    private CombatManager.GameStateChangedHandler resetBuffHandler;
    public void Destroy()
    {
        if (resetBuffHandler != null)
        {
            CombatManager.OnGameStateChanged -= resetBuffHandler;
        }
    }
    public override void Initialize()
    {
        lowerBound = 2;
        upperBound = 6;
        Speed = 1;

        myName = "Mutilate";
        description = "After on hit, the target gains a wound stack after taking damage for the rest of this turn.";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material;
        OriginalPosition = transform.position;
        base.Initialize();
    }

    //(@Author anrui) OnHit modifies how wound works fundamentally (Probably a bad idea now that I think of it...)
    public override void OnHit()
    {
        base.OnHit();

        StatusEffectModifyValueDelegate originalHandler = Target.GetBuffsOnHitHandler(Wound.buffName);
        StatusEffectModifyValueDelegate woundIncrementer = delegate (ref int damage) 
        {
            originalHandler(ref damage);
            if (damage > 0)
            {
                Target.AddStacks(Wound.buffName, 1); //After taking damage, target gains a stack of wound
            }
        };
        Target.SetBuffsOnHitHandler(Wound.buffName, woundIncrementer);

        this.resetBuffHandler = ResetBuffHandler;
        CombatManager.OnGameStateChanged += ResetBuffHandler;
        void ResetBuffHandler(GameState gameState)
        {
            if (gameState != GameState.FIGHTING)
            {
                CombatManager.OnGameStateChanged -= ResetBuffHandler;
                Target.SetBuffsOnHitHandler(Wound.buffName, originalHandler);
            }
        }
    }

}
