using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Silencer : PistolCards
{
    public override void ExecuteActionEffect()
    {

    }

    // Start is called before the first frame update
    public override void Start()
    {
        lowerBound = 1;
        upperBound = 4;
        base.Start();
        Speed = 3;
        Block = 2;

        myName = "Silencer";
        CardType = CardType.RangedAttack;
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;



    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void ApplyEffect()
    {

        DupInit();

        
        Origin.ApplyAllBuffsToCard(ref duplicateCard);

    }

    public override IEnumerator OnHit()
    {
        Origin.AddStacks(Accuracy.buffName, 2);
        yield return StartCoroutine(base.OnHit());
    }
}
