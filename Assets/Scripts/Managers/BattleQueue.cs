using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleQueue : MonoBehaviour
{
    public static BattleQueue BattleQueueInstance; 
    private SortedArray actionQueue = new SortedArray();
    public RectTransform bqContainer;
    public GameObject iconPrefab;
    public GameObject clashingPrefab;

#nullable enable

    void Awake()
    {
        if (BattleQueueInstance == null)
        {
            BattleQueueInstance = this;
        }
        else if (BattleQueueInstance != this)
        {
            Destroy(this); 
        }
    }

    public void AddAction(ActionClass action)
    {
        action.OnQueue();
        actionQueue.Insert(action);
        RenderBQ();
    }

    public void OnEnable()
    {
        CombatManager.PlayersWinEvent += ClearBattleQueue;
        CombatManager.EnemiesWinEvent += ClearBattleQueue;
    }

    public void OnDisable()
    {
        CombatManager.PlayersWinEvent -= ClearBattleQueue;
        CombatManager.EnemiesWinEvent -= ClearBattleQueue;
    }

    private void ClearBattleQueue()
    {
        actionQueue.Clear();
    }

    // Retrieves the deletedCard from the action queue and gives it back to the player who played it.
    public void DeletePlayerAction(ActionClass deletedCard)
    {
        deletedCard.OnRetrieveFromQueue();
        actionQueue.RemoveWrapperWithActionClass(deletedCard);
        RenderBQ();
        PlayerClass player = (PlayerClass) deletedCard.Origin;
        player.ReaddCard(deletedCard);
    }

    public bool CanInsertPlayerCard(ActionClass actionClass)
    {
        return actionQueue.IsUniqueSpeed(actionClass);
    }

    //Remove all cards with (@param entity) as the target and origin
    public void RemoveAllInstancesOfEntity(EntityClass entity)
    {
        actionQueue.RemoveAllInstancesOfEntity(entity);
    }

    /*  Renders the cards in List<GameObject> bq to the screen, as children of the bqContainer.
    */
    private void RenderBQ()
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
                ActionClass leftClashItem = battlingWrapper.PlayerAction!;
                ActionClass rightClashItem = battlingWrapper.EnemyAction!;
                clashingRenderedCopy.GetComponent<ClashingBattleQueueIcon>().renderClashingIcons(leftClashItem, rightClashItem);
                clashingRenderedCopy.transform.SetParent(bqContainer, false);
            } else
            {
                GameObject renderedCopy = Instantiate(iconPrefab, new Vector3(100, 100, -10), Quaternion.identity);
                renderedCopy.transform.SetParent(bqContainer, false);
                renderedCopy.GetComponent<BattleQueueIcons>().RenderBQIcon(battlingWrapper.GetTheOnlyExistingAction());
            }
        }
    }

    //Gives BattleQueue ownership of the lifetime of the Dequeue coroutine.
    public void BeginDequeue()
    {
        StartCoroutine(Dequeue());
    }

    // Starts emptying the battle queue and starting the fight.
    // MODIFIES: the actionQueue is progressively emptied until it is empty. 
    private IEnumerator Dequeue()
    {
        List<ActionWrapper> array = actionQueue.GetList();
        if (!(array.Count == 0))
        {
            CombatManager.Instance.GameState = GameState.FIGHTING;
        }
        while (!(array.Count == 0))
        {
            ActionWrapper actionWrapper = array[0];
            if (actionWrapper.IsClashing())
            {
                yield return StartCoroutine(CardComparator.Instance.ClashCards(actionWrapper.PlayerAction!, actionWrapper.EnemyAction!));
            } else
            {
                yield return StartCoroutine(CardComparator.Instance.OneSidedAttack(actionWrapper.GetTheOnlyExistingAction()));
            }
            array.Remove(actionWrapper); // this utilises the default method for lists 
            RenderBQ();
        }
        if (CombatManager.Instance.GameState == GameState.FIGHTING)
        {
            CombatManager.Instance.GameState = GameState.SELECTION;
        }
    }

    // Provide immutable copy of array
    internal List<ActionWrapper> ProvideArray()
    {
        return new(actionQueue.GetList());
    }

    // A sorted array implementation for ActionWrapper No duplicate speed invariants inherently added for flexibility in the future.
    // However, please call CanInsertCard if you want to prevent the player from inserting cards with dupicate speeds
    internal class SortedArray
    {
        private readonly List<ActionWrapper> array = new List<ActionWrapper>();

        public void Clear()
        {
            array.Clear();
        }
        public void Insert(ActionClass actionCard)
        {
            ActionWrapper insertingWrapper = CreateClashingWrapper(actionCard);
            array.Insert(LocationToInsertWrapper(insertingWrapper), insertingWrapper);
        }

        // Returns false if the player cannot insert a card into the queue due to duplicate speed.
        public bool IsUniqueSpeed(ActionClass actionCard)
        {
            foreach (ActionWrapper existingWrapper in array)
            {
                if (existingWrapper.HasPlayerAction() &&
                    existingWrapper.PlayerAction!.Origin is PlayerClass &&  // Restrict overlap to only apply to players, otherwise spawnables suck.
                    actionCard.Speed == existingWrapper.PlayerAction!.Speed)
                {
                    return false;
                }
            }
            return true;
        }

        //Searches for the first Empty Wrapper that can clash with (@param actionCard), making a new one if none exists
        //Returns Wrapper with the (@param actionCard) wrapped
        //Modifies: (@field array) as it will remove the existing wrapper from that array
        private ActionWrapper CreateClashingWrapper(ActionClass actionCard)
        {
            foreach (ActionWrapper existingWrapper in array)
            {
                if (existingWrapper.CanClashWithAction(actionCard))
                {
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
                if ((existingWrapper.HasPlayerAction() && (existingWrapper.PlayerAction!.Origin == entity || existingWrapper.PlayerAction!.Target == entity)) || 
                    (existingWrapper.HasEnemyAction() && (existingWrapper.EnemyAction!.Target == entity || existingWrapper.EnemyAction!.Origin == entity)))
                {
                    array.RemoveAt(i);
                }
            }
        }

        // Removes and returns Wrapper that contains (@param removedCard), null if it cant be found
        // If the removed ActionClass was clashing, then reinsert the other Clashing Card.
        public ActionWrapper? RemoveWrapperWithActionClass(ActionClass removedCard)
        {
            foreach (ActionWrapper existingWrapper in array)
            {
                if (existingWrapper.PlayerAction == removedCard || existingWrapper.EnemyAction == removedCard)
                {
                    array.Remove(existingWrapper);
                    if (existingWrapper.IsClashing())
                    {
                        // Restores the targetting of the enemy card if the card was retrieved by the player
                        if (existingWrapper.WasRetargetted())
                        {
                            existingWrapper.EnemyAction!.Target = existingWrapper.OriginalEnemysTarget;
                        }
                        Insert(existingWrapper.PlayerAction == removedCard ? existingWrapper.EnemyAction! : existingWrapper.PlayerAction!);
                    }
                    return existingWrapper;
                }
            }
            return null;
        }

        //Order in declaration determines tiebreaker in the event that wrappers share similar speeds. Higher up means more priority
        private enum WrapperType
        {
            Clashing,
            Player,
            Enemy 
        }

        private WrapperType GetWrapperType(ActionWrapper wrapper)
        {
            if (!wrapper.IsClashing() && wrapper.HasPlayerAction()) return WrapperType.Player;
            if (!wrapper.IsClashing() && wrapper.HasEnemyAction()) return WrapperType.Enemy;
            return WrapperType.Clashing;
        }

        // Returns the index in (@field array) that the (@param wrapper) should be inserted in based on its WrapperType
        private int LocationToInsertWrapper(ActionWrapper wrapper)
        {
            int firstPosition = 0;
            WrapperType insertingWrapperType = GetWrapperType(wrapper);

            foreach (ActionWrapper existingWrapper in array)
            {
                WrapperType existingWrapperType = GetWrapperType(existingWrapper);

                if (wrapper.ClashingSpeed < existingWrapper.ClashingSpeed)
                {
                    firstPosition++;
                }
                else if (wrapper.ClashingSpeed == existingWrapper.ClashingSpeed)
                {
                    if (insertingWrapperType <= existingWrapperType) // Compares their enum ordinal values
                    {
                        break; // If insertingWrapperType is higher up on the enum, it takes priority and we stop here.
                    }
                    else
                    {
                        firstPosition++; //If not keep going down the queue.
                    }
                }
            }
            return firstPosition;
        }

        public List<ActionWrapper> GetList()
        {
            return array;
        }
    }

    internal class ActionWrapper
    {
        public ActionClass? PlayerAction { get; private set; } 
        public ActionClass? EnemyAction { get; private set; }
        // Only non null when this wrapper is clashing. Represents the original enemy's target that may have been redirected by the player.
        public EntityClass? OriginalEnemysTarget { get; private set; }


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

        //Returns: whether (@param clashingAction) will clash with this wrapper.
        public bool CanClashWithAction(ActionClass clashingAction)
        {
            if (IsClashing()) return false;

            // If the PlayerAction's speed is greater than the enemy Action's speed it can redirect the enemy's attack and clash. Enemies cannot redirect player attacks. 
            bool playerWrapperClashesWithEnemyAction = PlayerAction != null && (PlayerAction.Origin == clashingAction.Target || PlayerAction.Speed >= clashingAction.Speed) && PlayerAction.Target == clashingAction.Origin;
            bool enemyWrapperClashesWithPlayerAction = EnemyAction != null && EnemyAction.Origin == clashingAction.Target && (EnemyAction.Target == clashingAction.Origin || clashingAction.Speed >= EnemyAction.Speed);

            if (clashingAction.IsPlayedByPlayer())
            {
                return enemyWrapperClashesWithPlayerAction;
            } else
            {
                return playerWrapperClashesWithEnemyAction;
            }
        }

        // Modifies: this to become a clashing wrapper.
        // Requires: That (@param clashingAction) can clash with an Action within this wrapper (Call ClashesWithAction first)
        public void SetClashingAction(ActionClass clashingAction)
        {
            if (!CanClashWithAction(clashingAction))
            {
                Debug.LogWarning("You called SetClashingAction with a wrapper that didnt actually clash. " +
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
            OriginalEnemysTarget = EnemyAction!.Target;
            EnemyAction.Target = PlayerAction!.Origin;
        }

        // Helper to return the only action that exists in this non clashing wrapper
        // Requires: This wrapper to not be clashing
        public ActionClass GetTheOnlyExistingAction()
        {
            if (IsClashing()) Debug.LogWarning("You called GetNonClashingChild on a wrapper that clashes, behaviour may be unexpected as I am returning the player action. " +
                "Here is my info: Action played by player " + PlayerAction?.GetName() + "Action played by enemy" + EnemyAction?.GetName());
            if (HasPlayerAction()) return PlayerAction!;
            return EnemyAction!;
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

        public bool WasRetargetted()
        {
            return OriginalEnemysTarget != null;
        }

        public override string ToString()
        {
            return "Wrapper has player: " + PlayerAction?.name + "Enemy: " + EnemyAction?.name;
        }
    }
}