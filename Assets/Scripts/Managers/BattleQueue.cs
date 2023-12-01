using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleQueue : MonoBehaviour
{ 

    public static BattleQueue battleQueue;
    private List<ActionClass> playerActions;
    private List<ActionClass> enemyActions;

    // Awake is called before Start.
    void Awake()
    {
        if (battleQueue == null)
        {
            battleQueue = this;
        }
        else if (battleQueue != this)
        {
            Destroy(battleQueue); // this is out of circumspection; unsure it this is even needed.
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // Notes for future it makes sense for the ActionClass to have an instance of BattleQueue.
    // That way they can automtically insert themselve herein and BattleQueue doesn't have to poll.
}
