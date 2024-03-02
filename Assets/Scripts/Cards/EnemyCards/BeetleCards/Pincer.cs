using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Pincer : BeetleAttacks
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
        Speed = 3;

        myName = "Pincer";
        description = "Scissors";
       
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
    }


    public override void OnHit()
    {
        base.OnHit();
        //StartCoroutine(AttackAnimation());
        //Origin.AttackAnimation("IsPounding");
    }

    // public IEnumerator AttackAnimation()
}
