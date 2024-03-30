using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static StatusEffect;

public class Mutilate : AxeCards
{
#nullable enable
    CombatManager.GameStateChangedHandler? GameStateChangedHandler;
    private void OnDestroy()
    {
        if (GameStateChangedHandler != null)
        {
            CombatManager.OnGameStateChanged -= GameStateChangedHandler;
        }
        EntityClass.OnEntityDeath -= HandleTargetDeath;
    }
    public override void Initialize()
    {
        lowerBound = 2;
        upperBound = 6;
        Speed = 3;

        myName = "Mutilate";
        description = "On hit, the target gains a wound stack. Then the target gains one wound each time they are staggered this turn.";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material;
        OriginalPosition = transform.position;
        base.Initialize();
        CardType = CardType.MeleeAttack;
    }

    void AddWound(int damage)
    {
        if (damage > 0)
        {
            Target.AddStacks(Wound.buffName, 1); //After taking damage, target gains a stack of wound
        }
    }

    //(@Author anrui) OnHit modifies how wound works fundamentally (Probably a bad idea now that I think of it...)
    public override void OnHit()
    {
        Target.EntityTookDamage += AddWound;
        CombatManager.OnGameStateChanged += ResetBuffHandler;
        EntityClass.OnEntityDeath += HandleTargetDeath;
        void ResetBuffHandler(GameState gameState)
        {
            if (gameState != GameState.FIGHTING)
            {
                CombatManager.OnGameStateChanged -= ResetBuffHandler;
                Target.EntityTookDamage -= AddWound;
            }
        }

        GameStateChangedHandler = ResetBuffHandler;
        base.OnHit();
    }

    void HandleTargetDeath(EntityClass entity)
    {
        if (entity == Target)
        {
            Target.EntityTookDamage -= AddWound;
        }
    }
}
