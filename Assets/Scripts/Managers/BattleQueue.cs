using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BattleQueue : MonoBehaviour
{ 

    public static BattleQueue BattleQueueInstance; // naming convention without the _
    private List<ActionClass> playerActions; // naming convention
    private List<ActionClass> enemyActions; // naming convention

    public EntityClass player;
    
    // Awake is called before Start.
    void Awake()
    {
        if (BattleQueueInstance == null)
        {
            BattleQueueInstance = this;
            playerActions = new List<ActionClass>();   
            enemyActions = new List<ActionClass>();
        }
        else if (BattleQueueInstance != this)
        {
            Destroy(BattleQueueInstance); // this is out of circumspection; unsure it this is even needed.
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        // needless
    }

    // to add an action to the playerActions list
    // REQUIRES: appropriate handling in the invoking superclass; note how the entity is INFERRED to be the player.
    // TO_UPDATE: for that speed thing Anrui specified.
    public void AddPlayerAction(ActionClass action)
    {
        
        playerActions.Add(action);
        action.Origin = player;

        CardComparator.Instance.CompareCards(action, action);
    }

 


    // FOR TESTING PURPOSES
    void AddRandomEnemyActions ()
    {
        // this requires understanding of a hierarchy accomplishable in a bit. 
    }

    // Update is called once per frame; This is so that Alissa's Highlight Manager can denote a a successful addition. 
    public void UpdateTest()
    {
        Debug.Log("Something has been added to PQ"); // Initial; will add details later.
    }
}

//INVALID ASSUMPTION DO NOT OMIT:
// Notes for future it makes sense for the ActionClass to have an instance of BattleQueue.
// That way they can automtically insert themselve herein and BattleQueue doesn't have to poll.

// DO NOT OMIT: 
// default access specifier for methods is different... Is that contingent on the variable type? 

