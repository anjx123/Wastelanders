using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.UI.Image;

public class FocusedDefense : StaffCards
{
    public override void ExecuteActionEffect()
    {

    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 2;
        upperBound = 4;

        Speed = 5;


        myName = "Focused Defense";
        description = "Gain 1 Focus For Every Attack The Clashing Opponent Makes Against This Character";
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
        //TODO: Search the BQ for every attack the clashing opponent makes against this character 
        base.ApplyEffect();
    }
}
