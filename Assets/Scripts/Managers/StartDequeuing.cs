using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDequeuing : MonoBehaviour
{

    // BQ reference not needed. 
    
    void OnMouseDown()
    {
        BattleQueue.BattleQueueInstance.Dequeue();
    }
}
