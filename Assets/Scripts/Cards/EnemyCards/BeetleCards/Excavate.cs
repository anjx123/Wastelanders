using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Excavate : BeetleAttacks
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
        Speed = 2;

        myName = "Excavate";
        description = "I'm gonna GET you";
        
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
    }


    public override void OnHit()
    {
        if (this.Target is Crystals) duplicateCard.actualRoll *= 2;
        base.OnHit(); // remove when attack animation is implemented
        //StartCoroutine(AttackAnimation());
        //Origin.AttackAnimation("IsDigging");
    }

    // public IEnumerator AttackAnimation()
}
