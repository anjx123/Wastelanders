using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.UI.Image;

public class CalmTheMind : StaffCards
{
    public override void OnCardStagger()
    {

    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 1;

        Speed = 4;
        
        myName = "Calm The Mind";
        description = "Gain 2 Stacks Of Focus, then block";
        
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
        base.Initialize();
        CardType = CardType.Defense;
    }

    // Update is called once per frame
    void Update()
    {
    }
    public override void ApplyEffect()
    {
        
        Origin.AddStacks(Focus.buffName, 2);
        base.ApplyEffect();
    }
}
