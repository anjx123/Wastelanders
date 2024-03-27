using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheapStrike : StaffCards
{
    public override void OnCardStagger()
    {

    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 3;
        
        Speed = 1;

        myName = "Cheap Strike";
        description = "If this card hits the opponent, gain 2 Focus";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
        CardType = CardType.MeleeAttack;
        base.Initialize();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void CardIsUnstaggered()
    {
        base.CardIsUnstaggered();
        Origin.AddStacks(Focus.buffName, 2);
    }

    public override void OnHit()
    {
        Vector3 diffInLocation = Target.myTransform.position - Origin.myTransform.position;
        Origin.UpdateFacing(diffInLocation, null);
        this.Target.TakeDamage(Origin, duplicateCard.actualRoll);
        CardIsUnstaggered();
    }
}
