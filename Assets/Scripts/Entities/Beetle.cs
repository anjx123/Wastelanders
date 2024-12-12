using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class Beetle : EnemyClass
{
    public const float BEETLE_SCALING = 0.6f;
    public delegate void GainedBuffsHandler(string buffType, int stacks, Beetle beetle); // queen should subscribe to this
    public static event GainedBuffsHandler OnGainBuffs;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        myName = "Beetle";
    }

    // Overrides the normal behaviour of adding buffs. Instead, broadcasts for the queen to handle
    public override void AddStacks(string buffType, int stacks)
    {
        base.AddStacks(buffType, stacks);
        OnGainBuffs?.Invoke(buffType, stacks, this);
    }
}
