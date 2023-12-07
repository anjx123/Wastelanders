using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.XR;
using static UnityEngine.GraphicsBuffer;

public class BattleQueue : MonoBehaviour
{ 

    public static BattleQueue BattleQueueInstance; // naming convention without the _
    private SortedArray actionQueue; // in the same queue
    // private List<ActionClass> enemyActions; // naming convention
    private static int SIZE = 20; // size of actionQueue, change if necessary
    public RectTransform bqContainer;

    public EntityClass player;
    
    // Awake is called before Start.
    void Awake()
    {
        if (BattleQueueInstance == null)
        {
            BattleQueueInstance = this;

            actionQueue = new SortedArray(SIZE);
            // playerActions = new List<ActionClass>();   
            // enemyActions = new List<ActionClass>(); 
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
        
        actionQueue.Insert(action);

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

     /*  Renders the cards in List<GameObject> hand to the screen, as children of the handContainer.
     *  Cards are filled in left to right.
     *  REQUIRES: Nothing
     *  MODIFIES: Nothing
     * 
     */
    // void RenderHand()
    // {
    //     List<ActionClass> hand = actionQueue.getList();

    //     for (int i = 0; i < hand.Count; i++)
    //     {
    //         hand[i].transform.SetParent(handContainer.transform, false);
    //         hand[i].transform.position = Vector3.zero;

    //         float distanceToLeft = handContainer.rect.width / 2 - (i * cardWidth);

    //         float y = handContainer.transform.position.y;
    //         Vector3 v = new Vector3(-distanceToLeft, y, 1);
    //         hand[i].transform.position = v;
    //         hand[i].gameObject
    //     }
    // }

// A sorted array implementation for ActionClass (cards)
public class SortedArray
{
    private List<ActionClass> array;
    private int size;

    public SortedArray(int capacity)
    {
        array = new List<ActionClass>(capacity);
        size = 0;
    }

    public void Insert(ActionClass card)
    {
        if (size == array.Count)
        {
            return;
        }

        int index = BinarySearch(card.getSpeed(), 0, size - 1, card.Origin);

        // Insert the new value at the correct position
        array.Insert(index, card);
        size++;
    }

    public void Remove(ActionClass card)
    {
        int index = BinarySearch(card.getSpeed(), 0, size - 1, card.Origin);

        if (index < size && array[index] == card)
        {
            // Remove the value at the specified index
            array.RemoveAt(index);
            size--;
        }
        // else: element not found
    }

    public int BinarySearch(int speed, int left, int right, EntityClass origin)
    {
        while (left <= right)
        {
            int mid = left + (right - left) / 2;

            if (array[mid].getSpeed() == speed && array[mid].Origin == origin)
            {
                return mid; // Element found
            }
            else if (array[mid].getSpeed() < speed)
            {
                left = mid + 1;
            }
            else
            {
                right = mid - 1;
            }
        }

        return left; // Element not found, return the position where it should be inserted
    }

    public int getSize()
    {
        return size;
    }

    public List<ActionClass> getList()
    {
        return array;
    }
}

}

//INVALID ASSUMPTION DO NOT OMIT:
// Notes for future it makes sense for the ActionClass to have an instance of BattleQueue.
// That way they can automtically insert themselve herein and BattleQueue doesn't have to poll.

// DO NOT OMIT: 
// default access specifier for methods is different... Is that contingent on the variable type? 
