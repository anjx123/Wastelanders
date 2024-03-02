using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fling : BeetleAttacks
{
    [SerializeField]
    private List<Sprite> animationFrame = new();
    public override void ExecuteActionEffect()
    {

    }


    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 4;
        base.Initialize();
        Speed = 4;

        myName = "Fling";
        description = "Watch Out";
        CardType = CardType.RangedAttack;
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
    }


    public override void OnHit()
    {
        base.OnHit();
        Origin.AddStacks(Resonate.buffName, 1);
        //StartCoroutine(AttackAnimation());
        //Origin.AttackAnimation("IsPounding");
    }

    // public IEnumerator AttackAnimation()
}
