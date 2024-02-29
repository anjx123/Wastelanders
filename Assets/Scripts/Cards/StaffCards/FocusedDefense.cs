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


        myName = "FocusedDefense";
        description = "Gain 1 Focus For Every Attack The Clashing Opponent Makes Against This Character";
        CardType = CardType.Defense;
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
        base.Initialize();


    }

    public override void ApplyEffect()
    {
/*        //TODO: Search the BQ for every attack the clashing opponent makes against this character 
        // card doesn't know who they're clashing with; player does. 
        // in essence: ask bq to search for an attack (i.e. there is a clash); if yes, reinsert THIS and gain focus, else retire.
        if (BattleQueue.BattleQueueInstance.SearchForClash(this.Origin))
        {
            Origin.AddStacks(Focus.buffName, 1);
            BattleQueue.BattleQueueInstance.InsertDupPlayerAction(this); // this works because the card itself is removed from the BQ before clash cards is called
        }*/

        // UPDATE: This cannot work as has been instructed since there is no way to detect a clash because the wrapper is removed before this is called. 
        base.ApplyEffect();
    }
}
