using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlurryDuplicate : Flurry
{
    public override void CardIsUnstaggered()
    {
        base.CardIsUnstaggered();
        Destroy(this.gameObject);
    }
}
