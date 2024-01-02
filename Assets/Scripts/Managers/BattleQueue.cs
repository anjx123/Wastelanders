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
    // private List<GameObject> enemyActions; // naming convention
    private static int SIZE = 100; // size of actionQueue, change if necessary
    public RectTransform bqContainer;
    public int cardWidth = 1;

    
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
            Destroy(this); // this is out of circumspection; unsure it this is even needed.
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
    public void AddPlayerAction(GameObject action)
    {
        actionQueue.Insert(action);
        UpdateTest(); // Initial; will add details later.
        RenderBQ();
        ClashBothEntities(action.GetComponent<ActionClass>().Origin, action.GetComponent<ActionClass>().Target);
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
        Debug.Log("Something has been added to BQ"); // Initial; will add details later.
    }

     /*  Renders the cards in List<GameObject> bq to the screen, as children of the bqContainer.
     *  Cards are filled in left to right.
     *  REQUIRES: Nothing
     *  MODIFIES: Nothing
     * 
     */
    void RenderBQ()
    {
        List<GameObject> hand = actionQueue.getList();

        foreach (Transform child in bqContainer.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < hand.Count; i++)
        {
            GameObject renderedCopy = Instantiate(hand[i], Vector3.zero, Quaternion.identity);
            renderedCopy.transform.SetParent(bqContainer, false);
            float cardHeight = bqContainer.rect.height;
            float scaleFactor = cardHeight / 9f;
            renderedCopy.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
            float distanceToLeft = bqContainer.rect.width / 2 - (i * cardWidth);
            float y = bqContainer.transform.position.y;
            Vector3 v = new Vector3(-distanceToLeft, y, 1);
            renderedCopy.transform.position = v;
            

            // hand[i].transform.SetParent(bqContainer.transform, false);
            // hand[i].transform.position = Vector3.zero;
            // float distanceToLeft = bqContainer.rect.width / 2 - (i * cardWidth);
            // float y = bqContainer.transform.position.y;
            // Vector3 v = new Vector3(-distanceToLeft, y, 1);
            // hand[i].transform.position = v;
        }
    }

// A sorted array implementation for GameObject (cards)
public class SortedArray
{
    private List<GameObject> array;

    public SortedArray(int capacity)
    {
        array = new List<GameObject>(capacity);
    }

    public void Insert(GameObject card)
    {
        int index = BinarySearch(card.GetComponent<ActionClass>().getSpeed(), 0, array.Count - 1, card.GetComponent<ActionClass>().Origin);

        // Insert the new value at the correct position
        array.Insert(index, card);
    }

    public void Remove(GameObject card)
    {
        int index = BinarySearch(card.GetComponent<ActionClass>().getSpeed(), 0, array.Count - 1, card.GetComponent<ActionClass>().Origin);

        if (index < array.Count && array[index] == card)
        {
            // Remove the value at the specified index
            array.RemoveAt(index);
        }
        // else: element not found
    }

    public int BinarySearch(int speed, int left, int right, EntityClass origin)
    {
        while (left <= right)
        {
            int mid = left + (right - left) / 2;

            // comparison to sort in descending order
            if (array[mid].GetComponent<ActionClass>().getSpeed() == speed && array[mid].GetComponent<ActionClass>().Origin == origin)
            {
                return mid; // Element found
            }
            else if (array[mid].GetComponent<ActionClass>().getSpeed() > speed) // Change from < to >
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

    public List<GameObject> getList()
    {
        return array;
    }
}


}

//INVALID ASSUMPTION DO NOT OMIT:
// Notes for future it makes sense for the GameObject to have an instance of BattleQueue.
// That way they can automtically insert themselve herein and BattleQueue doesn't have to poll.

// DO NOT OMIT: 
// default access specifier for methods is different... Is that contingent on the variable type? 
