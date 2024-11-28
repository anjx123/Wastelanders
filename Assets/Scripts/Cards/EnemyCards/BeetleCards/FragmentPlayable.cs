using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class FragmentPlayable : BeetleAttacks
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

        CostToAddToDeck = 2;

        description = "If this attack hits an enemy, gain +1 resonate";

        myName = "Fragment";
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
            AudioManager.Instance.PlaySFX(FRAGMENT_SOUND_EFFECT_NAME);
            base.OnHit();
        }
        Origin.AddStacks(Resonate.buffName, 1);
    }

    private void OnProjectileHit()
    {
        AudioManager.Instance.PlaySFX(FRAGMENT_SOUND_EFFECT_NAME);
        base.OnHit();
    }
}
