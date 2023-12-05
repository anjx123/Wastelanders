using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

        ClashBothEntities(action.Origin, action.Target);
    }

    /*
    EntityClass origin: Origin of the action card played
    EntityClass target: Target of the action card played

    Purpose: The two clashing enemies come together to clash, their positions will ideally be based off their speed
    Then, whoever wins the clash should stagger the opponent backwards. 
     */
    private void ClashBothEntities(EntityClass origin, EntityClass target)
    {
        //The Distance weighting will be calculated based on speeds of the two clashing cards
        Vector2 centeredDistance = (origin.myTransform.position * 0.3f + 0.7f * target.myTransform.position);
        float bufferedRadius = 0.6f;
        float duration = 0.6f;
        StartCoroutine(origin.MoveToPosition(centeredDistance, bufferedRadius, duration));
        StartCoroutine(target.MoveToPosition(centeredDistance, bufferedRadius, duration));

        StartCoroutine(GeneralTimer(duration + 1f, StaggerEntities, new KeyValuePair<EntityClass, EntityClass>(origin, target)));
    }

    /*
     * A Generic Timer function that performs a task when it is completed. 
     * A delegate is defined here and is essentially a function pointer to a call back. Yippee !!
     * 
     * float duration: Duration until function is called
     * TimerCompletedTask DoFinishedTask: A function pointer to a callback
     * object parameter: generic paramter to pass in to the delegate
     */
    private delegate void TimerCompletedTask(object parameter);
    private IEnumerator GeneralTimer(float duration, TimerCompletedTask DoFinishedTask, object parameter)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Debug.Log("Timer is up, finished task is executing");
        DoFinishedTask(parameter);
    }

    /*
     Takes in a pair of entities with Key being the origin and value being the Target
    Then it calculates a direction vector and staggers the Target bacl from the Origin.
     */
    private void StaggerEntities(object pair)
    {
        KeyValuePair<EntityClass, EntityClass> pairOfEntities = (KeyValuePair < EntityClass, EntityClass > ) pair;
        Vector2 directionVector = pairOfEntities.Value.myTransform.position - pairOfEntities.Key.myTransform.position;

        Vector2 normalizedDirection = directionVector.normalized;
        float staggerPower = 2f; //Depending on percentage health lost

        StartCoroutine(pairOfEntities.Value.StaggerBack(pairOfEntities.Value.myTransform.position + (Vector3) normalizedDirection * staggerPower));

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

