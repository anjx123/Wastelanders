using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;
public abstract class FrogAttacks : ActionClass
{
    [SerializeField] private AudioClip frogAttackSfx;
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
        StartCoroutine(projectileBehaviour.ProjectileAnimation(OnProjectileHit, Origin, Target));
    }

    private void OnProjectileHit()
    {
        MusicManager.Instance.PlaySFX(frogAttackSfx);
        base.OnHit();
    }
}