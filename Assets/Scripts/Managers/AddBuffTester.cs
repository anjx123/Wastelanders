using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddBuffTester : MonoBehaviour
{
    public List<GameObject> beetles = new();

    void OnMouseDown()
    {
        beetles[UnityEngine.Random.Range(0, beetles.Count)].GetComponent<EntityClass>().AddStacks(Accuracy.buffName, 2);
    }
}

