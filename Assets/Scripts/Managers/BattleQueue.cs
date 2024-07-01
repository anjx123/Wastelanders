using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class BattleQueue : MonoBehaviour
{

    public static BattleQueue BattleQueueInstance; // naming convention without the _
    private SortedArray actionQueue; // in the same queue
    // private List<GameObject> enemyActions; // naming convention
    public RectTransform bqContainer;
    public GameObject iconPrefab;
    public GameObject clashingPrefab;
    public GameObject combatInfoDisplay; // same display given to enemy combat card UI

#nullable enable
    // Awake is called before Start.
    void Awake()
    {
        if (BattleQueueInstance == null)
        {
            BattleQueueInstance = this;
            actionQueue = new SortedArray();
        }
        else if (BattleQueueInstance != this)
        {
            Destroy(this); // this is out of circumspection; unsure it this is even needed.
        }
    }

     public void DeletePlayerAction(ActionClass deletedCard)
    {
        actionQueue.RemoveWrapperWithActionClass(deletedCard);
        RenderBQ();
        PlayerClass player = (PlayerClass) deletedCard.Origin;
        player.ReaddCard(deletedCard);
    }

    public bool AddAction(ActionClass action)
    {
        bool didInsertSucceed = actionQueue.Insert(action);
        RenderBQ();
        return didInsertSucceed;
    }

    //Remove all cards with (@param entity) as the target and origin
    public void RemoveAllInstancesOfEntity(EntityClass entity)
    {
        actionQueue.RemoveAllInstancesOfEntity(entity);
    }


    /*  Renders the cards in List<GameObject> bq to the screen, as children of the bqContainer.
    *  Cards are filled in left to right.
    *  REQUIRES: Nothing
    *  MODIFIES: Nothing
    * 
    */
    void RenderBQ()
    {
        List<ActionWrapper> queue = actionQueue.GetList();

        foreach (Transform child in bqContainer.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < queue.Count; i++)
        {
            ActionWrapper battlingWrapper = queue[i];

            if (battlingWrapper.IsClashing())
            {
                GameObject clashingRenderedCopy = Instantiate(clashingPrefab, new Vector3(100, 100, -10), Quaternion.identity);
                ActionClass leftClashItem = battlingWrapper.PlayerAction!; //Non null because its clashing
                ActionClass rightClashItem = battlingWrapper.EnemyAction!; //Non null because its clashing
                clashingRenderedCopy.GetComponent<ClashingBattleQueueIcon>().renderClashingIcons(leftClashItem, rightClashItem);
                clashingRenderedCopy.transform.SetParent(bqContainer, false);
            } else
            {
                GameObject renderedCopy = Instantiate(iconPrefab, new Vector3(100, 100, -10), Quaternion.identity);
                renderedCopy.transform.SetParent(bqContainer, false);
                if (battlingWrapper.HasPlayerAction())
                {
                    renderedCopy.GetComponent<BattleQueueIcons>().RenderBQIcon(battlingWrapper.PlayerAction!);
                } else
                {
                    renderedCopy.GetComponent<BattleQueueIcons>().RenderBQIcon(battlingWrapper.EnemyAction!);
                }
            }
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
        List<ActionWrapper> array = actionQueue.GetList();
        if (!(array.Count == 0))
        {
            CombatManager.Instance.GameState = GameState.FIGHTING;
        }
        while (!(array.Count == 0))
        {
            ActionWrapper e = array[0];
            if (e.IsClashing())
            {
                yield return StartCoroutine(CardComparator.Instance.ClashCards(e.PlayerAction!, e.EnemyAction!));
            } else
            {
                ActionClass attackingAction;
                if (e.HasPlayerAction())
                {
                    attackingAction = e.PlayerAction!;
                } else
                {
                    attackingAction = e.EnemyAction!;
                }

                yield return StartCoroutine(CardComparator.Instance.OneSidedAttack(attackingAction));
            }
            array.Remove(e); // this utilises the default method for lists 
            RenderBQ();
        }
        if (CombatManager.Instance.GameState == GameState.FIGHTING)
        {
            CombatManager.Instance.GameState = GameState.SELECTION;
        }
    }

    // A sorted array implementation for ActionWrapper
    internal class SortedArray
    {
        private List<ActionWrapper> array;
        public SortedArray()
        {
            array = new List<ActionWrapper>();
        }


        // The speed invariant refers to 
        // prevent each player character from playing cards with duplicate speeds
        // Careful, because you could have multiple player characters that can have overlapping speeds
        // But one singular player character cannot have overlapping speeds
        public bool Insert(ActionClass actionCard)
        {
            foreach (ActionWrapper existingWrapper in array) // ensuring uniqueness of speed for one character inside the array
            {
                if (existingWrapper.HasPlayerAction() && 
                    actionCard.Speed == existingWrapper.PlayerAction!.Speed)
                {
                    return false; // don't insert. 
                }
            }

            ActionWrapper insertingWrapper = SearchForClasher(actionCard);
            int i = LocationToInsertWrapper(insertingWrapper);

            // else insert 
            array.Insert(i, insertingWrapper);
            return true;
        }

        //Searches for the first Empty Wrapper that clashes with (@param actionCard), making a new one if none exists
        //Returns Wrapper with the (@param actionCard) wrapped
        //Modifies: (@field array) as it will remove the existing wrapper from that array
        private ActionWrapper SearchForClasher(ActionClass actionCard)
        {
            foreach (ActionWrapper existingWrapper in array)
            {
                if (existingWrapper.ClashesWithAction(actionCard))
                {
                    Debug.Log("It clashes with the wrapper with:" + existingWrapper);
                    existingWrapper.SetClashingAction(actionCard);
                    array.Remove(existingWrapper);
                    return existingWrapper;
                }
            }

            return new ActionWrapper(actionCard);
        }

        //Removes all cards in the battle queue that have (@param entity) as the Origin or target.
        public void RemoveAllInstancesOfEntity(EntityClass entity)
        {
            for (int i = array.Count - 1; i >= 0; i--)
            {
                ActionWrapper existingWrapper = array[i];
                if ((existingWrapper.HasPlayerAction() && existingWrapper.PlayerAction!.Origin == entity) || (existingWrapper.HasEnemyAction() && existingWrapper.EnemyAction!.Target == entity))
                {
                    array.RemoveAt(i); //TODO: Should update so that player cards are returned if not used. 
                }
            }
        }

        //Removes and returns Wrapper that contains (@param removedCard), null if it cant be found
        //If the removed ActionClass is clashing, then reinsert the other Clashing Card.
        public ActionWrapper? RemoveWrapperWithActionClass(ActionClass removedCard)
        {
            foreach (ActionWrapper existingWrapper in array)
            {
                if (existingWrapper.PlayerAction == removedCard || existingWrapper.EnemyAction == removedCard)
                {
                    array.Remove(existingWrapper);
                    if (existingWrapper.IsClashing())
                    {
                        Insert(existingWrapper.PlayerAction == removedCard ? existingWrapper.EnemyAction! : existingWrapper.PlayerAction!);
                    }
                    return existingWrapper;
                }
            }
            return null;
        }


        //Order in declaration determines tiebreaker in the event that wrappers share similar speeds
        private enum WrapperType
        {
            Player,
            Clashing,
            Enemy 
        }

        private WrapperType GetWrapperType(ActionWrapper wrapper)
        {
            if (!wrapper.IsClashing() && wrapper.HasPlayerAction())
            {
                return WrapperType.Player;
            }
            else if (!wrapper.IsClashing() && wrapper.HasEnemyAction())
            {
                return WrapperType.Enemy;
            }
            else if (wrapper.IsClashing())
            {
                return WrapperType.Clashing;
            }
            else
            {
                throw new Exception("Invalid wrapper type check if both actions are null");
            }
        }

        private int LocationToInsertWrapper(ActionWrapper wrapper)
        {
            int firstPosition = 0;
            WrapperType newWrapperType = GetWrapperType(wrapper);

            foreach (ActionWrapper existingWrapper in array)
            {
                WrapperType existingWrapperType = GetWrapperType(existingWrapper);

                if (wrapper.ClashingSpeed < existingWrapper.ClashingSpeed)
                {
                    firstPosition++;
                }
                else if (wrapper.ClashingSpeed == existingWrapper.ClashingSpeed)
                {
                    if (newWrapperType <= existingWrapperType) //Compares if the newWrapperEnum is declared higher up in the enum than existingWrapper type
                    {
                        break; //If so, then it should be inserted in front of it
                    }
                    else
                    {
                        firstPosition++; //If not keep going down
                    }
                }
            }
            return firstPosition;
        }



        // NOTE: why are the attributes not lowerCamelCase? is it because of the syntactic sugar

        public List<ActionWrapper> GetList()
        {
            return array;
        }
    }

    internal class ActionWrapper
    {
        public ActionClass? PlayerAction { get; private set; } 
        public ActionClass? EnemyAction { get; private set; }
        public int ClashingSpeed
        {
            get
            {
                int playerSpeed = PlayerAction != null ? PlayerAction.Speed : 0;
                int enemySpeed = EnemyAction != null ? EnemyAction.Speed : 0;
                return Mathf.Max(playerSpeed, enemySpeed);
            }
        }

        //ActionWrapper can only be instantiated with one ActionClass 
        public ActionWrapper(ActionClass insertedAction)
        {
            if (insertedAction.IsPlayedByPlayer())
            {
                this.PlayerAction = insertedAction;
            } else
            {
                this.EnemyAction = insertedAction;
            }
        }

        //Returns: whether (@param clashingAction) will clash with any action that this wrapper wraps.
        public bool ClashesWithAction(ActionClass clashingAction)
        {
            if (HasEnemyAction() && HasPlayerAction()) return false;

            bool isTargettedByPlayer = PlayerAction != null && this.PlayerAction.Target == clashingAction.Origin && clashingAction.Target == PlayerAction.Origin;
            bool isTargettedByEnemy = EnemyAction != null && this.EnemyAction.Target == clashingAction.Origin && clashingAction.Target == EnemyAction.Origin;

            if (clashingAction.IsPlayedByPlayer())
            {
                return isTargettedByEnemy;
            } else
            {
                return isTargettedByPlayer;
            }
        }

        //Requires: That (@oaram clashingAction) clashes with an Action within this wrapper (Call ClashesWithAction first)
        public void SetClashingAction(ActionClass clashingAction)
        {
            if (!ClashesWithAction(clashingAction))
            {
                Debug.LogWarning("You tried to set clashingAction with a wrapper that didnt actually clash" +
                    "Here is my info: Action played by player " + PlayerAction?.GetName() + "Action played by enemy"+ EnemyAction?.GetName());
                return;
            }
            if (clashingAction.IsPlayedByPlayer())
            {
                PlayerAction = clashingAction;
            }
            else
            {
                EnemyAction = clashingAction;
            }
        }

        public bool IsClashing()
        {
            return PlayerAction != null && EnemyAction != null;
        }
        public bool HasPlayerAction()
        {
            return PlayerAction != null;
        }

        public bool HasEnemyAction()
        {
            return EnemyAction != null;
        }

        public override string ToString()
        {
            return "Wrapper has player: " + PlayerAction?.name + "Enemy: " + EnemyAction?.name;
        }

    }


}



//INVALID ASSUMPTION DO NOT OMIT:
// Notes for future it makes sense for the GameObject to have an instance of BattleQueue.
// That way they can automtically insert themselve herein and BattleQueue doesn't have to poll.

// DO NOT OMIT: 
// default access specifier for methods is different... Is that contingent on the variable type? 

