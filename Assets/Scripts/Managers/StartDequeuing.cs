using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDequeuing : MonoBehaviour
{

    // BQ reference not needed. 
    
    void OnMouseDown()
    {
        StartCoroutine(BattleQueue.BattleQueueInstance.Dequeue());
    }
}
