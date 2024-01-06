using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            actionQueue = new SortedArray(SIZE);
        }
        else if (BattleQueueInstance != this)
        {
            Destroy(this); // this is out of circumspection; unsure it this is even needed.
        }
    }


    // to add an action to the playerActions list
    // REQUIRES: appropriate handling in the invoking superclass; note how the entity is INFERRED to be the player.
    //           the queue is sorted
    // TO_UPDATE: for that speed thing Anrui specified.
    public bool AddPlayerAction(ActionClass action)
    {
        bool ret;
        if (!(actionQueue.Insert(action)))
        {
            //Debug.Log("BQ not Updated.");
            ret = false;
        }
        else
        {

            ret = true;
        }
        RenderBQ();
        return ret;
    }

    public void AddEnemyAction(ActionClass action, EntityClass origin)
    {
        action.Origin = origin;
        actionQueue.Insert(action);
        RenderBQ();
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
        }
    }

    //Gives BattleQueue ownership of the lifetime of the Dequeue coroutine.
    public void BeginDequeue()
    {
        StartCoroutine(Dequeue());
    }


    // Begins the dequeueing process. 
    // REQUIRES: An appropriate call. Note that this can be called even if the number of elements in the actionQueue is 0. Invariant array index 0 has largest speed. 
    // MODIFIES: the actionQueue is progressively emptied until it is empty. 
    public IEnumerator Dequeue()
    {
        List<ActionClass> array = actionQueue.GetList();
        bool beganFighting = false;
        if (!(array.Count == 0))
        {
            CombatManager.Instance.GameState = GameState.FIGHTING;
            beganFighting = true;
        }
        while (!(array.Count == 0))
        {
            ActionClass e = array[0];
            yield return StartCoroutine(CardComparator.Instance.ClashCards(e, e)); // essentially doing nothing. 
            array.Remove(e); // this utilises the default method for lists 
            RenderBQ();
            Debug.Log("An item hath been removed from the BQ");
        }
        if (beganFighting)
        {
            CombatManager.Instance.GameState = GameState.SELECTION;
        }
    }

    // A sorted array implementation for GameObject (cards)
    public class SortedArray
    {
        private List<ActionClass> array;
        // public int entryNumber; // no of actual entries inside the array. INVALID because binary search uses all 100 slots. <<<<< INVALID dynamic array always I think

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

        /*public void Insert(ActionClass card)
        {
            int index = BinarySearch(card.Speed, 0, array.Count - 1, card.Origin);

            // Insert the new value at the correct position
            array.Insert(index, card);
        }*/

        // The speed invariant refers to 
        // prevent each player character from playing cards with duplicate speeds
        // Careful, because you could have multiple player characters that can have overlapping speeds
        // But one singular player character cannot have overlapping speeds
        public bool Insert (ActionClass card)
        {
            int i = LinearSearch(card); // returns where to insert ensuring LIFO.

            // TODO manual checking need to generalise.
            //            if (card.Origin.GetName() == "Jackie" && i < array.Count && card.Speed != array[i].Speed)
            //            {
            //                array.Insert(i, card);
            //            } 
            

            // code for uniqueness inside an array. 
            
            // could be separate method. 
            if (i < array.Count) {
                for (int x = 0; x < array.Count; x++) // ensuring uniqueness of speed for one character inside the array
                {
                    if (array[x].IsPlayedByPlayer() && card.Speed == array[x].Speed)

                    {
                        return false; // don't insert. 
                    }
                }
            }

            // else insert 
            array.Insert(i, card);
            return true;

        }

        // remove methods are redundant??
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

        private void RemoveLinearSearch(ActionClass card)
        {
            int i = LinearSearch(card);

            if (i < array.Count && array[i] == card)  // reference comparison is ok here; follow the code usage; is used at dequeue; same instance.
            {
                array.RemoveAt(i);
            }
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

            return left; // Element not found, return the position where it should be inserted; implies ascending order. 
        }

        // essentially replaces binary search and is easier for LIFO insertion (Stack)
        // REQUIRES: the speed wherewith to intially sort the BQ and the apparent origin of the card
        // MODIFIES: nothing; modification is done (removal and addition) based on the calling function 
        // RETURNS:  the position to finally place the new action

        // NOTE: REMAINS: TESTING WITH ENEMY ACTIONS
        // NOTE: manual checking of the player entities and the enemy entities; this should not be the case; consider a change of party or perhaps a change in the enemy;
        //       need a dynamic system of generating the enemies (Andrew) and holding a reference herein and a system for keeping track of player party members. TODO

        // BQ is sorted like this: GREATER SPEED > SLOWER SPEED; and is discharged on this assumption as well; vide Dequeue 

        // INVARIANT: Players are always first and LIFO is maintained for both Enemies and Players; 
        private int LinearSearch(ActionClass card)
        {
            int elements = array.Count;
            int firstPosition = 0;
            for (int i = 0; i < elements; i++)
            {
                /*
if (card.Speed == array[i].Speed && !(card.Origin.GetName() == "Jackie"))// == array[i].Origin.GetName()) 
{
    firstPosition = i; // essentially if an enemy is first then insert player here (or insert enemy here LIFO maintained)
    break;

} else if (card.Speed == array[i].Speed) // kicks in later 
{
    firstPosition = i; // if an enemy is not first then LIFO for player
    break;
}
else 
*/
                if (card.Speed < array[i].Speed)
                {
                    firstPosition++;
                }
            }
            return firstPosition; // default should be the start if no cards exist of same speed or no cards at all exist. <<< Incorrect;
            // no cards at all exist.
            
        }
        // NOTE: why are the attributes not lowerCamelCase? is it because of the syntactic sugar

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
