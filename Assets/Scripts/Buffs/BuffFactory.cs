using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffFactory : MonoBehaviour
{
    // Returns a Buff of the Specified Type
    public static StatusEffect GetStatusEffect(string buffType)
    {
        switch (buffType)
        {
            case Accuracy.buffName:
                return new Accuracy();

            case Focus.buffName:
                return new Focus();

            case Resonate.buffName:
                return new Resonate();

            default:
                throw new System.Exception("Unkown Buff: " + buffType);
        }
    }
}
