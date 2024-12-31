using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.UI.Image;

public class Fling : BeetleAttacks, IPlayableBeetleCard
{
    [SerializeField] private ProjectileBehaviour projectileBehaviour;

    // Start is called before the first frame update
    public override void Initialize()
    {
        base.Initialize();
        lowerBound = 1;
        upperBound = 4;
        
        Speed = 3;

        description = "If this card hits an opponent, gain +1 resonate.";

        myName = "Fling";
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
    }

    private IEnumerator HandleProjectile()
    {
        yield return projectileBehaviour.ProjectileAnimation(Origin, Target);
        OnProjectileHit();
    }

    private void OnProjectileHit()
    {

        if (Target.Team == Origin.Team.OppositeTeam())
        {
            Origin.AddStacks(Resonate.buffName, 1);
        }
        AudioManager.Instance.PlaySFX(Fragment.FRAGMENT_SOUND_EFFECT_NAME);
        base.OnHit();
    }
}
