using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FrogAttacks : ActionClass
{
    public const string SPIT_SOUND_EFFECT_NAME = "Frog Hit";
    public const string FROG_ATTACK_NAME = "IsFrogging";
    public const string PRINCESS_FROG_ATTACK_NAME = "IsPrincessing";

    protected string frogAttackAnimationName = FROG_ATTACK_NAME;

    [SerializeField] private ProjectileBehaviour projectileBehaviour;


    public override void Initialize()
    {
        base.Initialize();
        CardType = CardType.RangedAttack;
    }

    public override void OnHit()
    {
        if (Origin.HasAnimationParameter(frogAttackAnimationName))
        {
            Origin.AttackAnimation(frogAttackAnimationName);
        }
        if (projectileBehaviour != null)
        {
            StartCoroutine(HandleProjectile());
        } else
        {
            AudioManager.Instance.PlaySFX(SPIT_SOUND_EFFECT_NAME); // temp sound effect.
            base.OnHit();
        }
    }
    private IEnumerator HandleProjectile()
    {
        // Most practical way to handle the delay for frog spitting before the projectile actually comes out. 
        yield return new WaitForSeconds(0.3f);
        yield return projectileBehaviour.ProjectileAnimation(Origin, Target);
        OnProjectileHit();
    }
    protected virtual void OnProjectileHit()
    {
        AudioManager.Instance.PlaySFX(SPIT_SOUND_EFFECT_NAME);
        base.OnHit();
    }
}