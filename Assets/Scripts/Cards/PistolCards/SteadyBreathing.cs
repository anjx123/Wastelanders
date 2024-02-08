using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.UI.Image;

public class SteadyBreathing : PistolCards
{
    public override void ExecuteActionEffect()
    {

    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 3;
        
        Speed = 5;
        Block = 2;
        
        myName = "SteadyBreathing";
        CardType = CardType.Defense;
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

        DupInit();

        Origin.AddStacks(Accuracy.buffName, 2);
        Origin.ApplyAllBuffsToCard(ref duplicateCard);

    }
}
