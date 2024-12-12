using System.Collections;
using UnityEngine;


public class Fragment : ActionClass, IPlayableQueenCard
{
    [SerializeField] private ProjectileBehaviour projectileBehaviour;

    public const string FRAGMENT_SOUND_EFFECT_NAME = "Queen Pierce";

    // Start is called before the first frame update
    public override void Initialize()
    {
        base.Initialize();
        lowerBound = 3;
        upperBound = 3;
        
        Speed = 5;

        description = "If this attack hits an opponent, gain +1 resonate";

        myName = "Fragment";
        CardType = CardType.RangedAttack;
        CostToAddToDeck = 2;
    }



    public override void OnHit()
    {
        if (Origin.HasAnimationParameter("IsShooting"))
        {
            Origin.AttackAnimation("IsShooting");
        }
        StartCoroutine(HandleProjectile());
        if (Target is PlayerClass) {
            Origin.AddStacks(Resonate.buffName, 1);
        }
    }

    private IEnumerator HandleProjectile()
    {
        yield return projectileBehaviour.ProjectileAnimation(Origin, Target);
        OnProjectileHit();
    }

    private void OnProjectileHit()
    {
        AudioManager.Instance.PlaySFX(FRAGMENT_SOUND_EFFECT_NAME);
        base.OnHit();
    }
}
