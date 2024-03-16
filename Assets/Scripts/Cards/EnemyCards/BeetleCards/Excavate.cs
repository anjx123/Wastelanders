using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.UI.Image;

public class Excavate : BeetleAttacks
{
    public override void ExecuteActionEffect()
    {

    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        base.Initialize();
        lowerBound = 1;
        upperBound = 4;
        
        Speed = 2;

        description = "Deals 2x damage to crystals.";

        myName = "Excavate";
        CardType = CardType.MeleeAttack;
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
    }


    // does 2x damage if target is crystal
    public override void OnHit()
    {
        if (Target is Crystals)
        {
            Vector3 diffInLocation = Target.myTransform.position - Origin.myTransform.position;
            Origin.UpdateFacing(diffInLocation, null);
            this.Target.TakeDamage(Origin, 2 * duplicateCard.actualRoll);
        }
        else
        {
            base.OnHit();
        }
    }
}
