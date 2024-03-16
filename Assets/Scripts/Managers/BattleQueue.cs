using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Profiling.Editor;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Assertions;
using static System.Collections.Specialized.BitVector32;
using static UnityEditor.Experimental.GraphView.GraphView;

public class BattleQueue : MonoBehaviour
{

    public static BattleQueue BattleQueueInstance; 
    private SortedArray protoQueue;
    private WrapperArray wrapperArray;
    // is an indicator of sorts; essentially if this is true what this means is that the round has started and all the enemy actions have been added. The player has to add a card.
    // upon the addition of the first card the enemy actions are FIRST tranmusted into wrappers and thereafter everything proceeds as intended. Is reset at the end of dequee. ASTER1
    // TODO low priority; would have to go to the combat manager subclass and invoke initial transmutation there but visually there is no distinction.
    private bool roundStart = true;


    // visuals:
    public RectTransform bqContainer;
    public readonly int cardWidth = 1;
    public GameObject iconPrefab;
    public GameObject clashingPrefab;

    #nullable enable //Turns on pedantic null checks, use exclamation mark (!) operator to assert non null and supress warnings.
    void Awake()
    {
        if (BattleQueueInstance == null)
        {
            BattleQueueInstance = this;
            protoQueue = new SortedArray();
            wrapperArray = new WrapperArray();
        }
        else if (BattleQueueInstance != this)
        {
            Destroy(this);
        }
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }


    // is referenced whenever game state changes to selection.
    public void TheBeginning()
    {
        RenderInitialBQ();
    }

    // for when the round has just started. 
    void RenderInitialBQ()
    {
        List<ActionClass> queue = protoQueue.GetList();

        foreach (Transform child in bqContainer.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < queue.Count; i++)
        {
            GameObject renderedCopy = Instantiate(iconPrefab, new Vector3(100, 100, -10), Quaternion.identity);
            renderedCopy.transform.SetParent(bqContainer, false);
            renderedCopy.GetComponent<BattleQueueIcons>().RenderBQIcon(queue[i]);
        }
        
    }


    // FOR ALL ADD METHODS: the Insert() method of SortedArray() is responsible for insertion into the WrapperArray as well. 


    // to add an action to the actionList and the wrapperArray
    // REQUIRES: the queue is sorted before invocation
    // AFTER: the player action may or not have been inserted; is still sorted regardless
    public bool AddPlayerAction(ActionClass action)
    {
        bool ret; // indicator for invoking method: if true should remove the card from the deck.
        if (!(protoQueue.Insert(action)))
        {
            //Debug.Log("BQ not Updated.");
            ret = false;
        }
        else
        {
            roundStart = false; // ASTER1
            ret = true;
        }
        RenderBQ(); 
        return ret;
    }


    // Removes the card if it is clicked on by the player whilst it is in the Queue. And then reinserts it into the issuing player's hand/deck. 
     public void DeletePlayerAction(ActionClass action)
    {
        Wrapper? w = wrapperArray.RemoveWrapperWithActionClass(action); // this builds new wrappers bear in mind. 
        ActionClass a = protoQueue.RemoveLinearSearch(action);
/*        if (w == null)
        {
            Debug.Log("Check Removal in Wrappers");
        }
        if (a == null)
        {
            Debug.Log("Check Removal in Array");
        }*/
        if (w == null || a == null)
        {
            throw new Exception("Logic is flawed. This method was called to delete an action that never existed.");
        }
        if (w.EnemyAction != null)
        {
            if (!wrapperArray.FindAvailablePlayerActionAndRedirect(w)) // if there is still a clash. 
            {
                w.EnemyAction.Target = w.ProtoEnemysTarget; 
            }
        }
        RenderBQ();
        PlayerClass? player = action.Origin as PlayerClass;
        player?.ReaddCard(action);
    }

    // to add an enemy action to the actionQueue and the WrapperArray
    public void AddEnemyAction(ActionClass action, EntityClass origin)
    {
        action.Origin = origin;
        protoQueue.Insert(action);
        RenderBQ(); 
    }

    // !!! TODO remove from the Wrapper Array as well 
    // @Author Anrui 
    //Remove all cards with (@param entity) as the target and origin
    public void RemoveAllInstancesOfEntity(EntityClass entity)
    {
        wrapperArray.RemoveAllInstancesOfEntity(entity);
        protoQueue.RemoveAllInstancesOfEntity(entity);
    }


    /*  Renders the cards in List<GameObject> bq to the screen, as children of the bqContainer.
    *  Cards are filled in left to right.
    *  REQUIRES: Nothing
    *  MODIFIES: Nothing
    * 
    */
    void RenderBQ()
    {
        List<Wrapper> wrappers = wrapperArray.GetWrappers();

        foreach (Transform child in bqContainer.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Wrapper wrapper in wrappers)
        {
            if (ClashCheck(wrapper))
            {
                GameObject clashingRenderedCopy = Instantiate(clashingPrefab, new Vector3(100, 100, -10), Quaternion.identity);
                ActionClass leftClashItem = wrapper.PlayerAction!;
                ActionClass rightClashItem = wrapper.EnemyAction!;
                clashingRenderedCopy.GetComponent<ClashingBattleQueueIcon>().renderClashingIcons(leftClashItem, rightClashItem);
                clashingRenderedCopy.transform.SetParent(bqContainer, false);
            } else
            {
                GameObject renderedCopy = Instantiate(iconPrefab, new Vector3(100, 100, -10), Quaternion.identity);
                renderedCopy.transform.SetParent(bqContainer, false);
                renderedCopy.GetComponent<BattleQueueIcons>().RenderBQIcon(wrapper.ReturnWhaYouHave());
            }
        }
    }    
   

    // returns true if there is a "Clash"
    // @param wrapper is the current wrapper being inspected;
    private bool ClashCheck(Wrapper wrapper)
    {
        return !wrapper.IsHalfEmpty();
    }

    // Gives BattleQueue ownership of the lifetime of the Dequeue coroutine.
    public void BeginDequeue()
    {
        // StartCoroutine(Dequeue());

        StartCoroutine(DequeueWrappers());
    }

    // Begins the dequeueing process. 
    // REQUIRES: An appropriate call. Note that this can be called even if the number of elements in the actionQueue is 0. Invariant array index 0 has largest speed. 
    // MODIFIES: the actionQueue is progressively emptied until it is empty. 
    // Note the positioning of the Remove from Wrappers/protoQueue: this is important for the dup cards invariant.
    public IEnumerator DequeueWrappers()
    {
        List<Wrapper> array = wrapperArray.GetWrappers();
        bool beganFighting = false;
        if (!(array.Count == 0))
        {
            CombatManager.Instance.GameState = GameState.FIGHTING;
            beganFighting = true;
        }

        int i = 0; // for debugging
        while (!(array.Count == 0))
        {
            if (i == 0)
            {
                i = 1;
            }
            Wrapper e = array[0];
            array.Remove(e); 
            wrapperArray.SortWrappers();

            // the same functionality is maintained; BattleQueue is responsible for the wrappers and dewrapping. 
            if (e.IsHalfEmpty())
            {
                ActionClass action = e.ReturnWhaYouHave();
                protoQueue.GetList().Remove(action); 
                yield return StartCoroutine(CardComparator.Instance.OneSidedAttack(action));
            }
            else
            {
                ActionClass pla = e.PlayerAction!;
                ActionClass ene = e.EnemyAction!;
                protoQueue.GetList().Remove(pla);
                protoQueue.GetList().Remove(ene);
                yield return StartCoroutine(CardComparator.Instance.ClashCards(pla, ene));
            }

            RenderBQ(); 

        }
        if (beganFighting)
        {
            CombatManager.Instance.GameState = GameState.SELECTION;
        }

        // ASTER1 
        roundStart = true;
    }

    // for duplicate enemy actions 
    public void InsertDupEnemyAction(ActionClass a)
    {
        protoQueue.InsertDupEnemyCard(a);
        RenderBQ(); 
    }

    // for duplicate/"special" player actions
    // Utilises the protoQueue's insert method as there is not need to redefine the method as the Invariant is upheld throughout
    // lifeline (BattleQueue itself never has two player actions by the same player entity together since the previous action is removed
    // vide Dequeu for above.
    public void InsertDupPlayerAction(ActionClass a)
    {
        // use just Insert as checks for initialisation of the dequeing process is redundant i.e. no call to AddPlayerAction 
        protoQueue.Insert(a);
        RenderBQ();
    }

    // A sorted array implementation for ActionClass.
    private class SortedArray
    {
        private List<ActionClass> array;

        // constructor for dynamic array
        public SortedArray()
        {
            array = new List<ActionClass>();
        }

        // The speed INVARIANT refers to 
        // prevent each player character from playing cards with duplicate speeds
        // Careful, because you could have multiple player characters that can have overlapping speeds
        // But one singular player character cannot have overlapping speeds

        // Insertion of an ActionClass (PlayerAction or EnemyAction) into the protoQueue and the wrapperArray
        // MODIFIES: this, wrapperArray
        // return value: whether or not the @param card was inserted 
        public bool Insert(ActionClass card) 
        {
            int i = LinearSearch(card); // returns where to insert ensuring LIFO.

            // ensuring uniqueness of speed for one character inside the array
            if (i < array.Count)
            {
                for (int x = 0; x < array.Count; x++) 
                {
                    if (array[x].IsPlayedByPlayer() && array[x].Origin == card.Origin && card.Speed == array[x].Speed)

                    {
                        return false; // don't insert. 
                    }
                }
            }

            // else insert:

            // this check here is necessary since all enemy actions are inserted before any player action and this is how this is
            // handled; this is arbitrary and can be refactored.
            if (card.IsPlayedByPlayer())
            {
                BattleQueueInstance.wrapperArray.InsertPlayerActionIntoWrappers(card);
                // initial enemy actions are added via transmutation so no invocation here. 
            }
            array.Insert(i, card); // the original order of the queue is ensured...
                                    // ASTER2 refer to note inside WrapperArray
            return true;

        }

        // has to be introduced because enemeies CAN now add actions after initial based on game conditions.
        public void InsertDupEnemyCard(ActionClass card)

        {
            int elements = array.Count;
            int firstPosition = 0;
            if (elements != 0)
            {
                for (int i = 0; i < elements; i++)
                {
                    if (card.Speed < array[i].Speed || (card.Speed == array[i].Speed && array[i].IsPlayedByPlayer()))
                    {
                        firstPosition++;
                    }
                }
            }
            array.Insert(firstPosition, card);
            BattleQueue.BattleQueueInstance.wrapperArray.InsertEnemyActionIntoWrappers(card);
            
        }

        //Removes all instances of an entity from the queue
        public void RemoveAllInstancesOfEntity(EntityClass entity)
        {
            for (int i = array.Count - 1; i >= 0; i--)
            {
                ActionClass actionClass = array[i];
                if (actionClass.Origin == entity || actionClass.Target == entity)
                {
                    array.RemoveAt(i); //TODO: Should update so that player cards are returned if not used
                }
            }

        }

        public ActionClass RemoveLinearSearch(ActionClass card)
        { 
            int elements = array.Count;
            if (elements != 0)
            {
                for (int i = 0; i < elements; i++)
                {
                    if (card == array[i]) // check for the reference 
                    {
                        array.RemoveAt(i);
                        return card;
                    }
                }
            }
            throw new Exception("Invalid Call to Remove");
        }

        // essentially replaces binary search and is easier for LIFO insertion (Stack)
        // REQUIRES: the speed wherewith to intially sort the BQ and the apparent origin of the card
        // MODIFIES: nothing; modification is done (removal and addition) based on the calling function 
        // RETURNS:  the position to finally place the new action

        // BQ is sorted like this: GREATER SPEED > SLOWER SPEED; and is discharged on this assumption as well; vide Dequeue 

        // INVARIANT: Players are always first and LIFO is maintained for both Enemies and Players; 
        private int LinearSearch(ActionClass card)
        {
            int elements = array.Count;
            int firstPosition = 0;
            if (elements != 0)
            {
                for (int i = 0; i < elements; i++)
                {
                    if (card.Speed < array[i].Speed)
                    {
                        firstPosition++;
                    }
                }
            }
            return firstPosition; 
                                    

        }

        public List<ActionClass> GetList()
        {
            return array;
        }
    }



    // Class for wrappers to be contained 
    private class WrapperArray
    {
        private List<Wrapper> wrappers = new List<Wrapper>();

        public WrapperArray()
        {
            // nothing here. 
        }

        // is called when roundStart is true. 
        // transforms all enemy actions into wrappers. 
        // is automatically sorted because iterating through a sorted array.

        // REQUIRES: the protoQueue is populated and roundStart is true;
        public void EnemyActionsTransformation()
        {
            for (int i = 0; i < BattleQueueInstance.protoQueue.GetList().Count; i++) 
            {
                ActionClass curAction = BattleQueueInstance.protoQueue.GetList()[i];
                Wrapper wrapper = new Wrapper(curAction);
                wrappers.Add(wrapper);
            }
            // DisplayWrapperArray();

            BattleQueue.BattleQueueInstance.roundStart = false;
        }

        // only called inside Insert() iff Insert() method of SortedArray is about to return true; not that it returns it; is called before the method ends. // ASTER2
        /* When a player plays a card against an enemy, the fastest action the enemy has will get promoted to the player�s speed. Even if the Enemy Attack is 
                targeting a different target. CASE 1 

            If the enemy�s speed is higher than the player�s speed. The player�s card gets promoted up instead. However, it only promotes player cards 
                that are targeting THEM, and will not promote attacks targeting different enemies. CASE 2 
            
            Case 1 V Case 2 == Case 1; in essence the speed doesn't matter, only the origin does. 
         */

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
            if (playerAct != null)
            {

                foreach (Wrapper curWrapper in wrappers)
                {
                    if (curWrapper.PlayerAction == null && curWrapper.EnemyAction != null) // if the wrapper is half-empty
                    {
                        if (playerAct.Target == curWrapper.EnemyAction.Origin) // where it's confirmed that the player has clicked the target like selected it. 
                        {
                            if (curWrapper.EnemyAction.Target != playerAct.Origin && curWrapper.EnemyAction.Speed > playerAct.Speed)
                            {
                                continue;
                            }
                            
                            curWrapper.PlayerAction = playerAct;

                            // The redirection occurs here because this method is invoked only when the player action can be successfully inserted; this code block's conditions are requisites as well.
                            if (curWrapper.ProtoEnemysTarget == null) // very important since this is the ORIGINAL target. 
                            {
                                curWrapper.ProtoEnemysTarget = curWrapper.EnemyAction.Target;
                                curWrapper.EnemyAction.Target = playerAct.Origin;
                            }
                            // redirection complete

                            curWrapper.Update();
                            SortWrappers();
                            // DisplayWrapperArray();
                            return;
                        }
                    }
                }
                // otherwise is a new wrapper; would happen if all enemy target actions are tied up BUT this does not ensure sorting by speed and player priority.
                Wrapper temp = new Wrapper(playerAct);
                temp.Update(); 
                wrappers.Add(temp);
                SortWrappers();
                // DisplayWrapperArray;
            }

        }

        // The clashing implementation here is
        /* When a player plays a card against an enemy, the fastest action the enemy has will get promoted to the player�s speed. Even if the Enemy Attack is 
        targeting a different target. CASE 1

        If the enemy�s speed is higher than the player�s speed. The player�s card gets promoted up instead. However, it only promotes player cards 
        that are targeting THEM, and will not promote attacks targeting different enemies. CASE 2 */
        // enemy card pairs with the highest speed player card that is targeting them.

        public void InsertEnemyActionIntoWrappers(ActionClass act)
        {
            if (act != null)
            {
                foreach (Wrapper curWrapper in wrappers)
                {
                    if (curWrapper.PlayerAction != null && curWrapper.EnemyAction == null && curWrapper.PlayerAction.Target == act.Origin) // speed is maintained 
                    {
                        curWrapper.EnemyAction = act;
                        curWrapper.Update();
                        SortWrappers();
                        // DisplayWrapperArray();
                        return;
                    }
                }
                // otherwise is a new wrapper
                Wrapper temp = new Wrapper(act);
                temp.Update(); 
                wrappers.Add(temp);
                SortWrappers();
                // DisplayWrapperArray();
            }
        }

        //Removes and returns Wrapper that contains (@param removedCard), null if it cant be found
        // method is for returning from the queue to the hand
        //If the removed ActionClass is clashing, then reinsert the other Clashing Card.
        public Wrapper? RemoveWrapperWithActionClass(ActionClass removedCard)
        {
            foreach (Wrapper existingWrapper in wrappers)
            {
                if (existingWrapper.PlayerAction == removedCard || existingWrapper.EnemyAction == removedCard)
                {
                    wrappers.Remove(existingWrapper);
                    if (existingWrapper.PlayerAction != null && existingWrapper.EnemyAction != null)
                    {
                        if (existingWrapper.PlayerAction == removedCard)
                        {
                            InsertEnemyActionIntoWrappers(existingWrapper.EnemyAction);
                        } else
                        {
                            InsertPlayerActionIntoWrappers(existingWrapper.PlayerAction);
                        }
                    }
                    return existingWrapper;
                }
            }
            return null;
        }

        //Removes all cards in the battle queue that have (@param entity) as the Origin or target.
        // for death
        public void RemoveAllInstancesOfEntity(EntityClass entity)
        {
            for (int i = wrappers.Count - 1; i >= 0; i--)
            {
                Wrapper existingWrapper = wrappers[i];
                if ((existingWrapper.PlayerAction != null && (existingWrapper.PlayerAction.Origin == entity || existingWrapper.PlayerAction.Target == entity)) || 
                    (existingWrapper.EnemyAction != null && (existingWrapper.EnemyAction.Target == entity || existingWrapper.EnemyAction.Origin == entity)))
                {
                    wrappers.RemoveAt(i); // would wanna remove all cards amiritie; NOTE: you could possibly transfer the attacks...
                }
            }
        }

        

        public void Swap(List<Wrapper> wrappers, int i, int j)
        {
            Wrapper temp = wrappers[i];

            wrappers[i] = wrappers[j];

            wrappers[j] = temp;
        }


        // called upon ALL insertions excepting the initial insertion of the enemy actions which relies on sorting done by protoQueue.
        // player priority is maintained
        // sorts the wrapperArray
        public void SortWrappers()
        {

            int count = wrappers.Count;

            if (count > 0)
                {
                // first sorting based on HighestSpeed
                // need to ensure LIFO order, however, so not equals to 
                for (int x = 0; x < count; x++)
                {
                    int y = x;
                    while (y > 0 && wrappers[y].HighestSpeed > wrappers[y - 1].HighestSpeed) // ok I know this is bubble sort but Insertiona and Selection both have worst case complexity of n2; though, arguably insertion is better here because the array is already sorted... 
                    {
                        Swap(wrappers, y - 1, y);
                        y--;
                    }
                }
            }


            // now sorting based on Player Priority when the highest speed is equal player wrapper comes first and fastest player comes first e.g. 5,4 and 4,5 and 2,5 and 3,5
            for (int x = 0; x < count; x++)
            {
                int y = x;
                while (y > 0 && wrappers[y].HighestSpeed == wrappers[y - 1].HighestSpeed) // moving from right to left with the left becoming increasingly shorter.
                {
                    if (wrappers[y].PlayerAction == null)
                    {
                        y--;
                        continue; // no swap needed 
                    }
                    else if (wrappers[y - 1].PlayerAction == null || wrappers[y]!.PlayerAction!.Speed > wrappers[y - 1].PlayerAction!.Speed)
                    {
                        Swap(wrappers, y - 1, y);
                    }
                    y--;
                }
            }
        }

        public List<Wrapper> GetWrappers()
        {
            return wrappers;
        }

        // REQUIRES: w.EnemyAction != null
        // note that the new clash has already been formed at this point. 
        // w is the discarded wrapper.
        public bool FindAvailablePlayerActionAndRedirect(Wrapper w)
        {
            foreach (Wrapper wrapper in wrappers)
            {
                if (wrapper.PlayerAction != null && wrapper.PlayerAction.Target == w.EnemyAction!.Origin && w.EnemyAction == wrapper.EnemyAction)
                {

                    if (w.ProtoEnemysTarget != null)
                    {
                        wrapper.ProtoEnemysTarget = w.ProtoEnemysTarget;
                    }
                    else if (wrapper.ProtoEnemysTarget == null)
                    {
                        wrapper.ProtoEnemysTarget = wrapper.EnemyAction.Target;
                    }
                    wrapper.EnemyAction.Target = wrapper.PlayerAction.Origin;
                    return true;
                }
            }
            return false;
        }
    }

    // Wrapper Element for WrapperArray;
    public class Wrapper
    {
        public ActionClass? PlayerAction { get; set; }
        public ActionClass? EnemyAction { get; set; }

        // This field is ONLY ever updated if a clash is introduced. It remains null until so. If a clash is inserted, it will retain information of the primary target until the round ends. Knowledge of this field should remain inside BQ.
        // Cannot see perfect access modifiers so as to obviate incorrect modification. 
        public EntityClass? ProtoEnemysTarget { get; set; } 

        public int HighestSpeed { get; set; } // used to sort the wrappers 
                                                // -1 indicates that the wrapper is empty 

        // Every enemy action is transformed into a single field wrapper after all the enemy actions have been inserted;
        public Wrapper(ActionClass action)
        {
            if (!action.IsPlayedByPlayer())
            {
                PlayerAction = null;
                this.EnemyAction = action;
                HighestSpeed = action.Speed;
            }
            else
            {
                this.PlayerAction = action;
                EnemyAction = null;
                HighestSpeed = action.Speed;
            }
        }

        // IS REDUNDANT NOW
        // returns the action with the highest speed. 
        // IS NOT RESPONSIBLE FOR THE DESTRUCTION AS THE DESTRUCTION MUST TAKE PLACE AFTER THE EXECUTION; 
        // updates Highest Speed. 
        public ActionClass ReturnHighest()
        {
            if (PlayerAction == null) // account for half wrapper. 
            {
                HighestSpeed = -1;
                ActionClass temp = EnemyAction!;
                EnemyAction = null;
                return temp;
            }
            else if (EnemyAction == null)
            {
                HighestSpeed = -1;
                ActionClass temp = PlayerAction;
                PlayerAction = null;
                return temp;
            }
            else
            {
                if (PlayerAction.Speed >= EnemyAction.Speed)
                {
                    HighestSpeed = EnemyAction.Speed;
                    ActionClass temp = PlayerAction;
                    PlayerAction = null;
                    return temp;
                }
                else
                {
                    HighestSpeed = PlayerAction.Speed;
                    ActionClass temp = EnemyAction;
                    EnemyAction = null;
                    return temp;
                }
            }
        }

        // REQUIRES: that it only has one element. Exception handling implemented. 
        // is intended for the rendering
        public ActionClass ReturnWhaYouHave()
        {
            if (PlayerAction != null && EnemyAction != null)
            {
                throw new Exception("Invalid call to method. Should only be called if one of the ActionClasses is emepty ");
            }
            return PlayerAction == null ? EnemyAction! : PlayerAction;
        }

        // Updates the HighestSpeed
        public void Update()
        {
            if (PlayerAction != null && EnemyAction != null)
            {
                HighestSpeed = PlayerAction.Speed >= EnemyAction.Speed ? PlayerAction.Speed : EnemyAction.Speed;
            }
            else if (PlayerAction == null && EnemyAction != null)
            {
                HighestSpeed = EnemyAction.Speed;
            }
            else if (EnemyAction == null && PlayerAction != null)
            {
                HighestSpeed = PlayerAction.Speed;
            }
            else
            {
                HighestSpeed *= -1;
            }
        }

        // returns true if the the wrapper is half empty; used for the displaying method.
        public bool IsHalfEmpty()
        {
            if (PlayerAction == null && EnemyAction == null)
            {
                throw new Exception("Why is there an empty wrapper?");
            }
            return PlayerAction == null || EnemyAction == null;
        }
    }
}
/*
 * TODO: Remove the protoQueue in the future to reduce coupling
 * TODO: If the player inserted nothing, Dequeue should bounce. Right now, no wrappers are present in the BQ entirely so it causes Dequeue to bounce.
 */ 
