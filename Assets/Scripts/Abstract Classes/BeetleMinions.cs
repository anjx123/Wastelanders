using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BeetleMinions : EnemyClass
{
    public delegate void ResonateGainHandler(string buffType, int stacks); 
    public static event ResonateGainHandler OnGainResonate;

    // Start is called before the first frame update
    public override void Start()
    {
       base.Start();
    }

    public override void AddStacks(string buffType, int stacks)
    {
        if (buffType == Resonate.buffName)
        {
            OnGainResonate?.Invoke(buffType, stacks);
        } else
        {
            base.AddStacks(buffType, stacks);
        }
    }
}
