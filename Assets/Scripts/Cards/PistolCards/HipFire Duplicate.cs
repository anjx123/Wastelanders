using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HipFireDuplicate : HipFire
{
    public override void CardIsUnstaggered()
    {
        base.CardIsUnstaggered();
        Destroy(this);
    }
}
