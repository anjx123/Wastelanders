using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class FlingPlayable : BeetleAttacks
{
    [SerializeField] private ProjectileBehaviour projectileBehaviour;

    // Start is called before the first frame update
    public override void Initialize()
    {
        base.Initialize();
        lowerBound = 1;
        upperBound = 4;
        Speed = 3;

        CostToAddToDeck = 2;

        description = "If this card hits an enemy, gain +1 resonate.";

        myName = "Fling";
        CardType = CardType.RangedAttack;
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
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
        }
        else
        {
            AudioManager.Instance.PlaySFX(Fragment.FRAGMENT_SOUND_EFFECT_NAME);
            base.OnHit();
        }
        if (Target is EnemyClass)
        {
            Origin.AddStacks(Resonate.buffName, 1);
        }
    }

    private void OnProjectileHit()
    {
        AudioManager.Instance.PlaySFX(Fragment.FRAGMENT_SOUND_EFFECT_NAME);
        base.OnHit();
    }
}
