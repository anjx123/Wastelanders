using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFireDuplicate : RapidFire
{
    // without ability
    // is Overriden to resolve MemoryLeak
    public override void CardIsUnstaggered()
    {
        base.CardIsUnstaggered();
        Destroy(this);
        // you've instantiated the prefab of the card and THAT needs to be destoryed: mem leak solved. 
    }
}
