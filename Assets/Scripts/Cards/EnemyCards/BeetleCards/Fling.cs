using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.UI.Image;

public class Fling : BeetleAttacks
{
    [SerializeField] private ProjectileBehaviour projectileBehaviour;

    // Start is called before the first frame update
    public override void Initialize()
    {
        base.Initialize();
        lowerBound = 1;
        upperBound = 4;
        
        Speed = 3;

        description = "If this card hits a player, gain +1 resonate.";

        myName = "Fling";
        CardType = CardType.RangedAttack;
        Renderer renderer = GetComponent<Renderer>();
    }



    public override void OnHit()
    {
        if (Origin.HasAnimationParameter("IsShooting"))
        {
            Origin.AttackAnimation("IsShooting");
        }
        StartCoroutine(projectileBehaviour.ProjectileAnimation(OnProjectileHit, Origin, Target));
        if (Target is PlayerClass) {
            Origin.AddStacks(Resonate.buffName, 1);
        }
    }

    private void OnProjectileHit()
    {
        AudioManager.Instance.PlaySFX(Fragment.FRAGMENT_SOUND_EFFECT_NAME);
        base.OnHit();
    }
}
