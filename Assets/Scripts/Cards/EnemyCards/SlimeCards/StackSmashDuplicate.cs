using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE: the duplicates are not inserted inside the decks as that does not make logical sense.
public class StackSmashDuplicate: StackSmash
{
    // is Overriden to resolve MemoryLeak
    public override void CardIsUnstaggered()
    {
        base.CardIsUnstaggered();
        Destroy(this.gameObject); 
        // you've instantiated the prefab of the card and THAT needs to be destoryed: mem leak solved. 
    }

}
