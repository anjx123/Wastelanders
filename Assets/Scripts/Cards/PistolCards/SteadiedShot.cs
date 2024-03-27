using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.UI.Image;

public class SteadiedShot : PistolCards
{
    public override void OnCardStagger()
    {

    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 5;
        Speed = 3;

        myName = "Steadied Shot";
        description = "Gain One Accuracy, then attack";
        CardType = CardType.RangedAttack;
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
        base.Initialize();


    }

    // Update is called once per frame
    void Update()
    {
    }
    public override void ApplyEffect()
    {
        
        Origin.AddStacks(Accuracy.buffName, 1);
        base.ApplyEffect();

    }
}
