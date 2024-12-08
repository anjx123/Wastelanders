using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Deprecated superclass, please do not use this any further
public abstract class SelectClass : MonoBehaviour
{
    protected string myName;
    public Transform myTransform;

    public string GetName() {
        return myName;
    }
}
