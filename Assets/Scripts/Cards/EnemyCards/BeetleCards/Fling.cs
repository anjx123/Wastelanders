using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.UI.Image;

public class Fling : BeetleAttacks
{
    [SerializeField] private ProjectileBehaviour projectileBehaviour;


    public override void ExecuteActionEffect()
    {

    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        base.Initialize();
        lowerBound = 1;
        upperBound = 4;
        
        Speed = 4;

        description = "If this card hits a player, gain +1 resonate.";

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
        StartCoroutine(projectileBehaviour.ProjectileAnimation(base.OnHit, Origin, Target));
        if (Target is PlayerClass) {
            Origin.AddStacks(Resonate.buffName, 1);
        }
    }
}
