using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuteredHaymaker : Haymaker
{
    // Start is called before the first frame update
    public override void Initialize()
    {
        base.Initialize();
        Speed = 3;
    }
}
