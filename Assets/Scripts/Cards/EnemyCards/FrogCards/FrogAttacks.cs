using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;
public abstract class FrogAttacks : ActionClass
{
    public const string SPIT_SOUND_EFFECT_NAME = "Frog Hit";
    [SerializeField] private ProjectileBehaviour projectileBehaviour;

    public override void Initialize()
    {
        base.Initialize();
        CardType = CardType.RangedAttack;
    }

    public override void OnHit()
    {
        if (Origin.HasAnimationParameter("IsShooting"))
        {
            Origin.AttackAnimation("IsShooting");
        }
        if (projectileBehaviour != null)
        {
            StartCoroutine(projectileBehaviour.ProjectileAnimation(OnProjectileHit, Origin, Target));
        } else
        {
            AudioManager.Instance.PlaySFX(SPIT_SOUND_EFFECT_NAME); // temp sound effect.
            base.OnHit();
        }
    }

    protected virtual void OnProjectileHit()
    {
        AudioManager.Instance.PlaySFX(SPIT_SOUND_EFFECT_NAME);
        base.OnHit();
    }
}