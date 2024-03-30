using UnityEngine;


public class Fragment : ActionClass
{
    [SerializeField] private ProjectileBehaviour projectileBehaviour;

    public override void OnCardStagger()
    {

    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        base.Initialize();
        lowerBound = 3;
        upperBound = 3;
        
        Speed = 5;

        description = "If this card hits a player, gain +1 resonate";

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
        StartCoroutine(projectileBehaviour.ProjectileAnimation(base.OnHit, Origin, Target));
        if (Target is PlayerClass) {
            Origin.AddStacks(Resonate.buffName, 1);
        }
    }
}
