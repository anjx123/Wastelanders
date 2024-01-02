using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
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
    public readonly int cardWidth = 1;
    public GameObject iconPrefab;


    // Awake is called before Start.
    void Awake()
    {
        if (BattleQueueInstance == null)
        {
            BattleQueueInstance = this;

            // dynamic implementation TODO
            // actionQueue = new SortedArray();
            
            // should not be static
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
    //           the queue is sorted
    // TO_UPDATE: for that speed thing Anrui specified.
    public void AddPlayerAction(ActionClass action)
    {
        actionQueue.Insert(action);
        UpdateTest(); // Initial; will add details later.
        RenderBQ();
        // StartCoroutine(CardComparator.Instance.ClashCards(action.GetComponent<ActionClass>(), action.GetComponent<ActionClass>()));
    }




    // FOR TESTING PURPOSES
    void AddRandomEnemyActions()
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
        List<ActionClass> hand = actionQueue.GetList();

        foreach (Transform child in bqContainer.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < hand.Count; i++)
        {
            GameObject renderedCopy = Instantiate(iconPrefab, Vector3.zero, Quaternion.identity);
            renderedCopy.GetComponent<SpriteRenderer>().sprite = hand[i].GetIcon();
            renderedCopy.transform.SetParent(bqContainer, false);
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


    // Begins the dequeueing process. 
    // REQUIRES: An appropriate call. Note that this can be called even if the number of elements in the actionQueue is 0. 
    // MODIFIES: the actionQueue is progressively emptied until it is empty. 
    public void Dequeue()
    {
        List<ActionClass> array = actionQueue.GetList();
        int count = array.Count;
        for (int i = 0; i < count; i++) // will iterate through all 100 entries; could replace array.Count with SIZE. Nope INVALID <<<<<< the array is dynamic
        {
            if (array[i] != null)
            {
                // at present there is not an allowance for card comparison since Alissa's code assumes uniqueness; VIDE Insert method in Sorted Array. 
                StartCoroutine(CardComparator.Instance.ClashCards(array[i].GetComponent<ActionClass>(), array[i].GetComponent<ActionClass>())); // essentially doing nothing. 
                // yield return new WaitForSeconds(1); // so that you can see the dequeuing happening; inserted in above Coroutine // might be useless because an animation exists tbh 
                array.Remove(array[i]);
                RenderBQ();
                Debug.Log("An item hath been removed from the BQ");
            }
        }
        // while(!array.empty()) 
        // Element e = arr[0];
        // arr.Remove(0);
    }

    // A sorted array implementation for GameObject (cards)
    public class SortedArray
    {
        private List<ActionClass> array;
        // public int entryNumber; // no of actual entries inside the array. INVALID because binary search uses all 100 slots. 

        // constructor for static array
        public SortedArray(int capacity)
        {
            array = new List<ActionClass>(capacity);
            // entryNumber = 0;
        }

        // constructor for dynamic array
        public SortedArray()
        {
            array = new List<ActionClass>();
        }

        public void Insert(ActionClass card)
        {
            int index = BinarySearch(card.Speed, 0, array.Count - 1, card.Origin);

            // Insert the new value at the correct position
            array.Insert(index, card);
        }

        public void Remove(ActionClass card)
        {
            int index = BinarySearch(card.Speed, 0, array.Count - 1, card.Origin);

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
                if (array[mid].Speed == speed && array[mid].Origin == origin)
                {
                    return mid; // Element found
                }
                else if (array[mid].Speed > speed) // Change from < to >
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

        public List<ActionClass> GetList()
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
