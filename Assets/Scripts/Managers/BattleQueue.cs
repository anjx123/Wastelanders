using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
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


    // to add an action to the actionList
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
            roundStart = false; // ASTER1
            ret = true;
        }
        RenderBQ(); // TODO 
        return ret;
    }

    public void AddEnemyAction(ActionClass action, EntityClass origin)
    {
        action.Origin = origin;
        actionQueue.Insert(action);
        RenderBQ(); // TODO

        // instead of having a tranmuatation method, could have it so that insertion of the enemy action from the get-go causes an instantiation of wrapper. 
    }





    /*  Renders the cards in List<GameObject> bq to the screen, as children of the bqContainer.
    *  Cards are filled in left to right.
    *  REQUIRES: Nothing
    *  MODIFIES: Nothing
    * 
    *  NEEDS TO BE UPDATED FOR THE WRAPPERARRAY !!! TODO
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
        bool beganFighting = false; 
        if (!(array.Count == 0))
        {
            CombatManager.Instance.GameState = GameState.FIGHTING;
            beganFighting = true;
        }
        while (!(array.Count == 0))
        {
            Wrapper e = array[0]; // concurrent modification problem is mitigated against

            ActionClass action = e.ReturnHighest(); // accounts for half-wrappers too 

            // %%% could be optimised for logic
            // removing from the Wrapper based on type
            if (action.IsPlayedByPlayer()) // for consistency's sake; does not impact logic as the rest depends on HighestSpeed and ReturnHighest()
            {
                e.PlayerAction = null;
                e.Update();
                Debug.Log("A player action of speed x is removed: " +  action.Speed);
            } else
            {
                e.EnemyAction = null;
                e.Update();
                Debug.Log("An enemy action of speed x is removed: " + action.Speed);
            }

            if (e.HighestSpeed == -1)
            {
                if (e.EnemyAction != null || e.PlayerAction != null)
                {
                    throw new Exception("Wrapper Invariant not Upheld"); // $$$ what are the implications for the coroutine and the exception? 
                }
                wrapperArray.GetWrappers().Remove(e); // wrapper is now worthless
            } else // the wrapper has a component left 
            {
                if (action.IsPlayedByPlayer())
                {
                    // insert enemy action 
                    wrapperArray.InsertEnemyActionIntoWrappers(e.EnemyAction);
                    wrapperArray.GetWrappers().Remove(e); // the above method creates a new wrapper regardless
                }
                else
                {
                    // insert player action
                    wrapperArray.InsertPlayerActionIntoWrappers(e.PlayerAction);
                    wrapperArray.GetWrappers().Remove(e); // the above method creates a new wrapper regardless
                }
            }

            wrapperArray.SortWrappers(); // circumspection

            // the same functionality is maintained; BattleQueue is responsible for the wrappers and dewrapping. 
            yield return StartCoroutine(CardComparator.Instance.ClashCards(action, action)); // $$$ the coroutine suspends it right?

            RenderBQ(); // !!! need to make a new render method. BUT before that need to rely on GameState for intial trigger of wrappers
            
            actionQueue.GetList().Remove(action);

        }
        if (beganFighting)
        {
            CombatManager.Instance.GameState = GameState.SELECTION;
        }

        // ASTER1 
        roundStart = true; 
        actionQueue.GetList().Clear(); // just for circumspection
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

        // ASTER1 
        roundStart = true;    
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
        public bool Insert(ActionClass card) // for both enemy and player actions
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
            array.Insert(i, card); // the original order of the queue is ensured...
            // ASTER2
            if (card.IsPlayedByPlayer()) { 
                BattleQueueInstance.wrapperArray.InsertPlayerActionIntoWrappers(card);
            } else
            {
                // initial enemy actions are added via transmutation.
            }
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
            // DisplayWrapperArray();

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

        // only called inside Insert() iff Insert() method of SortedArray is about to return true; not that it returns it; is called before the method ends. // ASTER2
        /* When a player plays a card against an enemy, the fastest action the enemy has will get promoted to the player’s speed. Even if the Enemy Attack is 
                targeting a different target. CASE 1

            If the enemy’s speed is higher than the player’s speed. The player’s card gets promoted up instead. However, it only promotes player cards 
                that are targeting THEM, and will not promote attacks targeting different enemies. CASE 2 */
        // if the above two are conditions are not germane create a new wrapper.
        public void InsertPlayerActionIntoWrappers(ActionClass playerAct)
        {
            // implementation is based on the fact that we are not checking already clashing entities. i.e. first enemy action is doled out. 
            // do perform check for availability

            // since this is only called if there is a successful insertion
            if (BattleQueue.BattleQueueInstance.roundStart)
            {
                EnemyActionsTransformation(); // implemented therein roundStart = false too;
            }

            foreach (Wrapper curWrapper in wrappers) 
            {
                if (curWrapper.PlayerAction == null) // if the wrapper is half-empty
                {
                    if (playerAct.Target == curWrapper.EnemyAction.Origin && curWrapper.EnemyAction.Speed > playerAct.Speed) // CASE 2; the card in wrapper is by the targeted enemy and ITS speed is greater; relying on reference equality 
                    {
                        curWrapper.PlayerAction = playerAct;
                        curWrapper.Update();
                        SortWrappers();
                        DisplayWrapperArray();
                        return;
                    } else if (playerAct.Target == curWrapper.EnemyAction.Origin)// CASE 1; 
                    {
                        curWrapper.PlayerAction = playerAct;
                        curWrapper.Update();
                        SortWrappers();
                        DisplayWrapperArray();
                        return;
                    }
                    // the paths represent the conditionals but in reality its just the same thing imo; 
                } 
            }
            // otherwise is a new wrapper; would happen if all enemy target actions are tied up BUT this does not ensure sorting by speed.
            Wrapper temp = new Wrapper(playerAct, false);
            temp.Update(); // this was reduddant..........
            wrappers.Add(temp);
            SortWrappers();
            DisplayWrapperArray(); // debugging 

        }

        // The clashing implementation here is
        /* When a player plays a card against an enemy, the fastest action the enemy has will get promoted to the player’s speed. Even if the Enemy Attack is 
        targeting a different target. CASE 1

        If the enemy’s speed is higher than the player’s speed. The player’s card gets promoted up instead. However, it only promotes player cards 
        that are targeting THEM, and will not promote attacks targeting different enemies. CASE 2 */
        // enemy card pairs with the highest speed player card that is targeting them.

        public void InsertEnemyActionIntoWrappers(ActionClass act)
        {
            foreach (Wrapper curWrapper in wrappers)
            {
                if (curWrapper.PlayerAction != null && curWrapper.PlayerAction.Target == act.Origin) // speed is maintained 
                {
                    curWrapper.EnemyAction = act;
                    curWrapper.Update();
                    SortWrappers();
                    DisplayWrapperArray();
                }
            }
            // otherwise is a new wrapper
            Wrapper temp = new Wrapper(act, true);
            temp.Update(); // this was redundant.....
            wrappers.Add(temp);
            SortWrappers();
            DisplayWrapperArray();
        }


        // Prints contents to Console
        public void DisplayWrapperArray()
        {

            foreach(Wrapper curWrapper in wrappers)
            {
                Debug.Log("$$$$$$");
                Debug.Log(curWrapper.PlayerAction == null ? "EMPTY " : curWrapper.PlayerAction.Speed);
                Debug.Log(curWrapper.EnemyAction == null ? "EMPTY " : curWrapper.EnemyAction.Speed);
            }

            Debug.Log("----------");
        }


        // NOTE: alternative implementation: rely on the reference in ActionQueue and conduct a search here to remove INVALID still need to display.
        // Called inside the InitialInsert
        // Called inside DequeueWrappers() each time a wrapper is dequed. 
        // player priority is maintained
        // (consider two player actions of the same speed in which one is wrapped) dequeue should ensure that the wrappers are automatically updated and sorted
        // via an updated insert method that relies on a generalised clash.
        // via appropriate calls to Insert (and therein Sort).
        // REQUIRES: size is at least one. need a check in Dequeu ASTER7
        public void SortWrappers()
        {
            // when dealing with arrays always look for con mod
            // sort based on speed and priority.
            int count = wrappers.Count;
            if (count > 0) {
                for (int x = 0; x < count; x++)
                {
                    for (int y = x; y > -1; y--) // TRANSFORM THIS INTO A WHILE LOOP. 
                    {
                        if (wrappers[x].HighestSpeed >= wrappers[y].HighestSpeed && PlayerPriority(wrappers[x], wrappers[y])) // for when player speed is the greatest regardless of emptiness
                        {
                            Wrapper temp = wrappers[x];
                            wrappers.Remove(wrappers[x]);
                            wrappers.Insert(y, temp); // insertion doesn't remove them per se so need a temp 
                            // break is Invalid; need to compare with all 
                        } else if (wrappers[x].PlayerAction != null && wrappers[y].PlayerAction != null && wrappers[y].PlayerAction.Speed > wrappers[x].PlayerAction.Speed
                            && wrappers[y].HighestSpeed == wrappers[x].HighestSpeed) // for 2,5 ; 3,5; for when you have to swap certain isues 
                        {
                            Wrapper temp = wrappers[x];
                            wrappers.Remove(wrappers[x]);
                            wrappers.Insert(y, temp);
                        } else if (wrappers[x].HighestSpeed > wrappers[y].HighestSpeed) { // among enemy actions still allowing for via ^ pp 
                            // if the enemy speed is greater than the paired or absent player action still move
                            Wrapper temp = wrappers[x];
                            wrappers.Remove(wrappers[x]);
                            wrappers.Insert(y, temp);
                        }
                    }
                }
            }
        }

        // Subfunction for SortWrappers
        private bool PlayerPriority(Wrapper wrapper1, Wrapper wrapper2)
        {
            return wrapper2.PlayerAction == null; // i.e. is an enemy action 
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
        // -1 indicates that the wrapper is empty 
        // bad encapsulation as maintenance is done outside.

        // Every enemy action is transformed into a single field wrapper after all the enemy actions have been inserted;
        // Will implement this via a field checker (roundStart) at present; should be implemented elsewhere for decorum. 

        // the true corresponds to enemy action; false to player action
        // TO REFACTOR !!! isEnemys
        public Wrapper(ActionClass action, bool isEnemys)
        {
            if (isEnemys)
            {
                PlayerAction = null;
                this.EnemyAction = action;
                HighestSpeed = action.Speed;
            } else
            {
                this.PlayerAction = action;
                EnemyAction = null;
                HighestSpeed = action.Speed;
            }
        }

        // returns the action with the highest speed. 
        // IS NOT RESPONSIBLE FOR THE DESTRUCTION AS THE DESTRUCTION MUST TAKE PLACE AFTER THE EXECUTION; for destruction check speed. 
        // call in dequeue. 
        // updates Highest Speed. 
        public ActionClass ReturnHighest()
        {
            if (PlayerAction == null) // account for half wrapper. 
            {
                HighestSpeed = -1;
                return EnemyAction; 
            } else if (EnemyAction == null)
            {
                HighestSpeed = -1;
                return PlayerAction;
            } else 
            {
                HighestSpeed = PlayerAction.Speed >= EnemyAction.Speed ? PlayerAction.Speed : EnemyAction.Speed;
                return PlayerAction.Speed >= EnemyAction.Speed ? PlayerAction : EnemyAction;
            }
         }

        // Updates the HighestSpeed
        // Requires that at least one is non null
        public void Update()
        {
            if (PlayerAction != null && EnemyAction != null)
            {
                HighestSpeed = PlayerAction.Speed >= EnemyAction.Speed ? PlayerAction.Speed : EnemyAction.Speed;
            } else if (PlayerAction == null)
            {
                HighestSpeed = EnemyAction.Speed;
            } else
            {
                HighestSpeed = PlayerAction.Speed;
            }
        }
    }


    // >>> so at the beginning of a round enemy actions are added automatically to the battle queue; assume that it is inserted correctly. 


}

// base.SuperMethod()

// done with the insertion of enemy actions 

// initial insertion of player action 
//          delete original wrapper and insert new wrapper OR keep original wrapper and cause it to be updated and then sort the array itself 

// for dequeue:
//          is responsible for ensuring player priority (i.e. after ReturnHighest()
//          after that perform a check on the wrapper based on speed. (but first check if the wrapper is empty or not anf based on that)
//          reinvoke the clashing method abstract it. 
//          call sort 
//          loop
// empty the actionarray at the end. 
//          
//          no need for new wrapper
//          update wrapper highest speed 
//          need to sort the wrappers anew 
// need a sort method that relies on the HighestSpeed attribute, maintain player priority


// query: when you use syntactic sugar for a member do you forego its initialisation outside the constructor; i.e. at declaration


// .getType == TypeOf; what Type is a type 
// throw new NotImplementedException();

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

// Ctrl D for duplicate 

// so only inner classes have to refer to the instance itself for static contexts; i.e. an inner class would have to use the class name with static variables/nmethod 