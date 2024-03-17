using System.Collections.Generic;
using UnityEngine;

public class HighlightManager : MonoBehaviour // later all entity highlighter
{

    public static HighlightManager Instance { get; private set; }
#nullable enable
    public static EntityClass? currentHighlightedEnemyEntity = null;
    public static ActionClass? currentHighlightedAction = null;
    public static PlayerClass? selectedPlayer = null;
    public RectTransform handContainer;
    private int CARD_WIDTH = 2;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
    }
    private void Start()
    {
        CombatManager.OnGameStateChanged += ResetSelection;
    }

    private void OnDestroy()
    {
        CombatManager.OnGameStateChanged -= ResetSelection;
    }

    public void OnEntityClicked(EntityClass clicked)
    {
        if (CombatManager.Instance.GameState != GameState.SELECTION) return;
        bool isOutlined = false;

/*        Debug.Log(clicked.GetType());
        Debug.Log(selectedPlayer);*/

        if (clicked is PlayerClass)
        {
            if ((PlayerClass)clicked != selectedPlayer)
            {
                currentHighlightedAction?.DeHighlight();
                currentHighlightedAction = null;
                UnRenderHand();
            }
            selectedPlayer = (PlayerClass)clicked;
            RenderHand(selectedPlayer.Hand);
        }
        else
        {
            if (selectedPlayer == null && clicked is EnemyClass) // don't need to check for highlighted action
            {
                PopUpNotificationManager.Instance.DisplayWarning(PopupType.SelectPlayerFirst);
                // no call to PQueue.
            }
            else if (currentHighlightedAction == null)
            {
                PopUpNotificationManager.Instance.DisplayWarning(PopupType.SelectActionFirst);
                // no call to PQueue. 
            }
            else if (currentHighlightedEnemyEntity == null && clicked is EnemyClass)
            {
                currentHighlightedEnemyEntity = clicked;
                currentHighlightedEnemyEntity.Highlight();
                isOutlined = true;
                // no call to PQueue.
            }
            else if (currentHighlightedEnemyEntity != clicked && clicked is EnemyClass)
            {
                currentHighlightedEnemyEntity?.DeHighlight();
                clicked.Highlight();
                currentHighlightedEnemyEntity = clicked;
                isOutlined = true;
                // no call to PQueue. 
            }
            else if (clicked is EnemyClass && currentHighlightedEnemyEntity != null)
            {
                isOutlined = currentHighlightedEnemyEntity.Toggle();
                // no call to PQueue.
            }
        }

        if (currentHighlightedEnemyEntity != null && currentHighlightedAction != null && isOutlined)
        {
            if (selectedPlayer == null)
            {
                throw new System.Exception("There has been a logical flaw in the preceding conditional set. You should never have currentHighlightedAction without a PlayerSelected.");
            }

            // note a tricky case here: you select a player, and an action from that player's deck. You select another player.
            // Now you select an enemy. Who issued the action?; To counter this vide the setting of highlighted action to null at the beginning of the function

            currentHighlightedAction.Target = currentHighlightedEnemyEntity;
            
            // currentHighlightedAction.Origin = selectedPlayer;
            // the preceding line of code is redundant (look at Initalisation of PlayerClass) and incorrect (see above)

            bool wasAdded = BattleQueue.BattleQueueInstance.AddPlayerAction(currentHighlightedAction); 

            currentHighlightedEnemyEntity.DeHighlight();
            currentHighlightedAction.DeHighlight();
            if (selectedPlayer != null && wasAdded) // you would NEED a selected player here. look in present code block
            {
                selectedPlayer.HandleUseCard(currentHighlightedAction);
            } else
            {
                PopUpNotificationManager.Instance.DisplayWarning(PopupType.SameSpeed);
            }
            currentHighlightedEnemyEntity = null;
            currentHighlightedAction = null;   
        }
    }

    public static void OnActionClicked(ActionClass clicked)
    {
        if (CombatManager.Instance.GameState != GameState.SELECTION) return;
        if (selectedPlayer != null)
        {
            if (currentHighlightedAction == null)
            {
                currentHighlightedAction = clicked;
                currentHighlightedAction.Highlight();
            }
            else if (currentHighlightedAction != clicked)
            {
                currentHighlightedAction.DeHighlight();
                clicked.Highlight();
                currentHighlightedAction = clicked;
            }
            else
            {
                if (!currentHighlightedAction.Toggle()) // if enemy chosen but no card chosen
                {
                    currentHighlightedAction = null;
                    if (currentHighlightedEnemyEntity != null)
                    {
                        currentHighlightedEnemyEntity.DeHighlight();
                    }

                }
            }
        }
    }

    private void ResetSelection(GameState gameState)
    {
        if (gameState == GameState.FIGHTING)
        {
            //Untoggle card if it is still selected when entering fighting
            if (currentHighlightedAction != null && !currentHighlightedAction.Toggle())
            {
                currentHighlightedAction = null;
                if (currentHighlightedEnemyEntity != null)
                {
                    currentHighlightedEnemyEntity.DeHighlight();
                }

            }
            UnRenderHand(); // NOTE: selectedPlayer should logically never be null here as you never initiate Fighting without playing a card and you can never have an unselected player after making a selection.
        } 
    }

    // Note that there should only be one instance per round in which selectedPlayer is null, hence the non-assertion. (initial player selection) 
    // Auto shifts to relevant player
    public void RenderHandIfAppropriate(PlayerClass player)
    {
        if (player == null)
        {
            throw new System.Exception("This method was called from an invalid location or there is a logic conundrum in OnEntityClicked");
        }

        UnRenderHand();
        selectedPlayer = player; 
        RenderHand(player.Hand);
    }

    /*  Renders the cards in List<GameObject> hand to the screen, as children of the handContainer.
    *  Cards are filled in left to right.
    *  REQUIRES: Nothing
    *  MODIFIES: Nothing
    * 
    */
    public void RenderHand(List<GameObject> hand)
    {
        for (int i = 0; i < hand.Count; i++)
        {
            hand[i].transform.SetParent(handContainer.transform, false);
            hand[i].transform.position = Vector3.zero;

            float distanceToLeft = (float)(handContainer.rect.width / 2 - (i * CARD_WIDTH));

            float y = handContainer.transform.position.y;
            Vector3 v = new Vector3(-distanceToLeft, y, -i);
            hand[i].transform.position = v;
            hand[i].transform.rotation = Quaternion.Euler(0, 0, -5);
        }
        RenderText(hand);
    }

    // Renders the information (text) of each card inside the player's hand. 
    private void RenderText(List<GameObject> hand)
    {
        for (int i = 0; i < hand.Count; i++)
        {
            hand[i].GetComponent<ActionClass>().UpdateDup();
        }
    }
    

    // "unrenders" the hand or more explictly:
    // " Moves the player's cards off-screen to hide them.
    // Note that this doesn't disable the card objects. "

    public void UnRenderHand()
    {
        List<GameObject> hand = new List<GameObject>();
        foreach (Transform child in handContainer)
        {
            hand.Add(child.gameObject);
        }

        for (int i = 0; i < hand.Count; i++)
        {
            hand[i].transform.position = new Vector3(-10, -10, -10);
        }
    }
    
}
