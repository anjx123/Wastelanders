using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class BattleQueue : MonoBehaviour
{

    public static BattleQueue BattleQueueInstance; // naming convention without the _
    private SortedArray actionQueue; // in the same queue
    // private List<GameObject> enemyActions; // naming convention
    private static int SIZE = 100; // size of actionQueue, change if necessary
    public RectTransform bqContainer;
    public readonly int cardWidth = 1;
    public GameObject iconPrefab;


    // is an indicator of sorts; essentially if this is true what this means is that the round has started and all the enemy actions have been added. The player has to add a card.
    // upon the addition of the first card the enemy actions are FIRST tranmusted into wrappers and thereafter everything proceeds as intended. Is reset at the end of dequee. ASTER1
    private bool roundStart = true;

    // the new wrapper array
    private WrapperArray wrapperArray;


    // Awake is called before Start.
    void Awake()
    {
        if (BattleQueueInstance == null)
        {
            BattleQueueInstance = this;
            actionQueue = new SortedArray(SIZE);
            wrapperArray = new WrapperArray();
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
            roundStart = false; // !!!!!!
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
        // StartCoroutine(Dequeue());

        StartCoroutine(DequeueWrappers());
    }

    // Begins the dequeueing process. 
    // REQUIRES: An appropriate call. Note that this can be called even if the number of elements in the actionQueue is 0. Invariant array index 0 has largest speed. 
    // MODIFIES: the actionQueue is progressively emptied until it is empty. 
    public IEnumerator DequeueWrappers()
    {
        List<Wrapper> array = wrapperArray.GetWrappers();
        bool beganFighting = false; // ASTER1: I have no idea how this got here but it renders the flag redundant 
        if (!(array.Count == 0))
        {
            CombatManager.Instance.GameState = GameState.FIGHTING;
            beganFighting = true;
        }
        while (!(array.Count == 0))
        {
            Wrapper e = array[0]; // concurrent modification problem is mitigated against

            // yield return StartCoroutine(CardComparator.Instance.ClashCards(e.PlayerAction, e.EnemyAction)); // For now, I'm assuming thta Clash Cards does things normally except for NULL cases which I'll have to understand first for appropriate logic 
            // for null could just pass in the same card twice seems to get it done tbh: 
            // yield return StartCoroutine(CardComparator.Instance.ClashCards((e.PlayerAction) ? e.PlayerAction : e.EnemyAction, (e.EnemyAction) ? e.EnemyAction : e.PlayerAction)); // ? : note to self: RETURNS the result of the following expression

            // array.Remove(e); >>> TO DO HIGHLY IMPORTANT
            // array.Remove(e); // right now IT IS REMOVING THE ENTIRE WRAPPER NEED TO FIX. cannot REMOVE ANY ITEMS RN will throw a null pointer exception

            // RenderBQ();
            // Debug.Log("An item hath been removed from the BQ"); // 


            // new logic: 
            ActionClass initial = (e.PlayerAction) ? e.PlayerAction : e.EnemyAction; // Because the player gets priority

            // yield return WaitForSeconds(1); // WHY NOT !!!!!!
            yield return null; 

            // >>>>>>>>>>>>>>>>>>>>>>>>>>>>> I;m still very confused about what they mean by Register a Clash: the action itself should determine the emephasis; this behaviour should be inside the ClashComparator Class

            // >>>>>>>>>>>>>>>>>>>>>>>>>>>>> maybe ClashCards should itself return the wrapper to return and should have a chnage in parameters. 
            // >>>>>>>>>>>>>>>>>>>>>>>>>>>>> explicate this maybe I'm missing something because of how I differ from your suggested implementaion

        }
        if (beganFighting)
        {
            CombatManager.Instance.GameState = GameState.SELECTION;
        }

        // ASTER1 
        roundStart = true;    // so only inner classes have to refer to the instance itself...
        // TO REMOVE: this is so that the battle queue is emptied itdelf:
        // Dequeue();
    }


    // Begins the dequeueing process. 
    // REQUIRES: An appropriate call. Note that this can be called even if the number of elements in the actionQueue is 0. Invariant array index 0 has largest speed. 
    // MODIFIES: the actionQueue is progressively emptied until it is empty. 
    public IEnumerator Dequeue()
    {
        List<ActionClass> array = actionQueue.GetList();
        bool beganFighting = false; // ASTER1: I have no idea how this got here but it renders the flag redundant 
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

        // ASTER1 
        roundStart = true;    // so only inner classes have to refer to the instance itself...
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
            // ASTER2
            BattleQueueInstance.wrapperArray.InitialInsertIntoWrapperArray(card);
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



    // Class for wrappers to be contained 
    public class WrapperArray
    {
        private List<Wrapper> wrappers = new List<Wrapper>();

        public WrapperArray()
        {
            // nothing here. 
        }

        // is called when roundStart is true. 
        // transforms all enemy actions into wrappers. 
        // is automatically sorted because iterating through a sorted array.

        // peep the redundancy is referncing and calling
        public void EnemyActionsTransformation()
        {
            for (int i = 0; i < BattleQueueInstance.actionQueue.GetList().Count; i++) // !!! why was BattleQueueInstance necessary here; object reference for non static field? 
            {
                ActionClass curAction = BattleQueueInstance.actionQueue.GetList()[i];
                Wrapper wrapper = new Wrapper(curAction, true); // funnily enough this won't work in just C; works here because new is dynamic allocation.
                wrappers.Add(wrapper);
            }

            BattleQueue.BattleQueueInstance.roundStart = false;

            // Deactivate the actionQueue;
            // this is flawed at present and I see no point in removing this as it is already implemented; reimplementing or refactoring this functionality should come later
            // for reference notice the implementaion of methods inside *this* class assume implementation of that.
/*            while (BattleQueueInstance.actionQueue.GetList().Count != 0)
            {
                BattleQueueInstance.actionQueue.GetList().Remove(BattleQueueInstance.actionQueue.GetList()[0]);
            }
            // alternatively
            BattleQueueInstance.actionQueue.GetList().Clear();*/
        }

        // only called iff Insert() method of SortedArray is about to return true; not that it returns it; is called before the method ends. // ASTER2
        /* When a player plays a card against an enemy, the fastest action the enemy has will get promoted to the player’s speed. Even if the Enemy Attack is 
                targeting a different target. CASE 1

            If the enemy’s speed is higher than the player’s speed. The player’s card gets promoted up instead. However, it only promotes player cards 
                that are targeting THEM, and will not promote attacks targeting different enemies. CASE 2 */
        // if the above two are conditions are not germane create a new wrapper.
        // this is called initial insert since this relies on the invariant being held by the actionQueue itself. 
        public void InitialInsertIntoWrapperArray(ActionClass playerAct)
        {
            // implementation is based on the fact that we are not checking already clashing entities. i.e. first enemy action is doled out. 
            // do perform check for availability

            // since this is only called if there is a successful insertion
            if (BattleQueue.BattleQueueInstance.roundStart)
            {
                EnemyActionsTransformation(); // roundStart = false too;
            }

            foreach (Wrapper curWrapper in wrappers)
            {
                if (curWrapper.PlayerAction == null) // if the wrapper is half-empty
                {
                    if (playerAct.Target == curWrapper.EnemyAction.Origin && curWrapper.EnemyAction.Speed > playerAct.Speed) // CASE 2; the card in wrapper is by the targeted enemy and ITS speed is greater; relying on reference equality 
                    {
                        curWrapper.PlayerAction = playerAct;
                        return;
                    } else if (playerAct.Target == curWrapper.EnemyAction.Origin)// CASE 1; 
                    {
                        curWrapper.PlayerAction = playerAct; 
                        return;
                    }
                    // the paths represent the conditionals but in reality its just the same thing imo; 
                } 
            }
            // otherwise is a new wrapper; would happen if all enemy target actions are tied up 
            wrappers.Add(new Wrapper(playerAct, false));
        }

        // TODO: insert into wrapperArray
        // IS NOT RESPONSIBLE FOR SORTING; 
        // REQUIRES: a half-empty wrapper

        // this is very iffy since this does not allow for re-pairing after the initial pairing. Could be remedied...
        public void Insert(Wrapper act) 
        {
/*            if (act.IsPlayedByPlayer())
            {
                // 
            }*/
            wrappers.Add(act);
        }

        // Prints contents to Console
        public void DisplayWrapperArray()
        {
            // hi; TODO
        }

        // Called at the end of and inside Insert(...) each time;
        public void SortWrappers()
        {
            //
        }

        public List<Wrapper> GetWrappers()
        {
            return wrappers;
        }
    }

    // Wrapper Element for WrapperArray;
    public class Wrapper
    {
        public ActionClass PlayerAction { get; set; }
        public ActionClass EnemyAction { get; set; }
        
        public int HighestSpeed { get; set; } // used to sort the wrappers 

        // Every enemy action is transformed into a single field wrapper after all the enemy actions have been inserted;
        // Will implement this via a field checker (roundStart) at present; should be implemented elsewhere for decorum. 

        // the true corresponds to enemy action; false to player action
        public Wrapper(ActionClass action, bool isEnemys)
        {
            if (isEnemys)
            {
                PlayerAction = null;
                this.EnemyAction = action; // TODO checK; isCorrect; ref names
            } else
            {
                this.PlayerAction = action;
                EnemyAction = null; 
            }
        }

        // returns the action with the highest speed. 
        // IS NOT RESPONSIBLE FOR THE DESTRUCTION AS THE DESTRUCTION MUST TAKE PLACE AFTER THE EXECUTION
        public ActionClass ReturnHighest()
        {
            if (PlayerAction == null) // account for half wrapper. 
            {
                return EnemyAction; 
            } else if (EnemyAction == null)
            {
                return PlayerAction;
            } else 
            {
                return PlayerAction.Speed >= EnemyAction.Speed ? PlayerAction : EnemyAction;
            }
         }
    }


    // >>> so at the beginning of a round enemy actions are added automatically to the battle queue; assume that it is inserted correctly. 


}


// query: when you use syntactic sugar for a member do you forego its initialisation outside the constructor; i.e. at declaration


//INVALID ASSUMPTION DO NOT OMIT:
// Notes for future it makes sense for the GameObject to have an instance of BattleQueue.
// That way they can automtically insert themselve herein and BattleQueue doesn't have to poll.

// DO NOT OMIT: 
// default access specifier for methods is different... Is that contingent on the variable type? 


// public EntityClass Target { get; set; }
// start coroutine (function returning IEnumerator; has a yield return (IEnumerator)) can be nested; see coroutine implementation in BattleQueue. 
// start coroutine is inside a non-IEnumerator returner. 
// Distinction betweeen array and fields. 
// TODO: check if fields are instantiated before the Awake method
// naming conventions 
// .Count 
// .Remove() 
// .Add()
// ctrl shift / for block comment 
// ctrl R for rename
// foreach (Wrapper curWrapper in wrappers)
// { get; set; }
// public ActionClass PlayerAction { get; set; } effectively useless as they rely on the original modifier; i.e. if set to private wouldn't matter
// == is for reference equality
// == for strings is different
// case statmetn in c#

// ctrl up down for moevemnt 
// atl for moving lines up and doww; wonder what navigating the functions shortcut is 
// ctrl r stands for rename 

// Add and insert equivalents in Java !!!!

// ctrl shif / for block