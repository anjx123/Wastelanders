using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class Trip : StaffCards
{
    public override void OnCardStagger()
    {

    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 3;

        Speed = 2;

        myName = "Trip";
        description = "If this attack is unstaggered, gain 2 Focus";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
        CardType = CardType.MeleeAttack;
        base.Initialize();
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
