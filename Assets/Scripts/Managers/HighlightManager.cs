using System.Collections.Generic;
using System.Xml.Linq;
using UI_Elements;
using UnityEngine;

public class HighlightManager : MonoBehaviour // later all entity highlighter
{

    public static HighlightManager Instance { get; private set; }
    public RectTransform handContainer;
    public Transform deckContainer;
    public DeckRemainderViewController deckRemainderViewController;
#nullable enable
    public EntityClass? currentHighlightedEnemyEntity = null;
    public ActionClass? currentHighlightedAction = null;
    public PlayerClass? selectedPlayer = null;

    public delegate void HighlightEventDelegate(EntityClass e);
    public event HighlightEventDelegate? EntityClicked;
    public delegate void ActionAddedDelegate(ActionClass card);
    public event ActionAddedDelegate? PlayerManuallyInsertedAction;

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
        EntityClass.OnEntityClicked += OnEntityClicked;
        ActionClass.CardClickedEvent += OnActionClicked;
    }

    private void OnDestroy()
    {
        CombatManager.OnGameStateChanged -= ResetSelection;
        EntityClass.OnEntityClicked -= OnEntityClicked;
        ActionClass.CardClickedEvent -= OnActionClicked;

        EntityClicked = null;
        PlayerManuallyInsertedAction = null;
    }


    public void OnEntityClicked(EntityClass clicked)
    {
        if (CombatManager.Instance.GameState != GameState.SELECTION || PauseMenu.IsPaused) return;
        EntityClicked?.Invoke(clicked);

        if (clicked is PlayerClass clickedPlayer)
        {
            HandlePlayerClick(clickedPlayer);
        }
        else if (clicked is EnemyClass clickedEnemy)
        {
            HandleEnemyClick(clickedEnemy);
        }
    }

    public void SetActivePlayer(PlayerClass? forcedPlayer)
    {
        if (forcedPlayer != selectedPlayer)
        {
            ResetCurrentHighlightedAction();
        }
        selectedPlayer = forcedPlayer;
        if (forcedPlayer != null) RenderHand(forcedPlayer.Hand, forcedPlayer.Pool);
    }
    private void HandlePlayerClick(PlayerClass clickedPlayer)
    {
        if (clickedPlayer != selectedPlayer)
        {
            ResetCurrentHighlightedAction();
            selectedPlayer = clickedPlayer;
            RenderHand(clickedPlayer.Hand, clickedPlayer.Pool);
        }
    }

    private void ResetCurrentHighlightedAction()
    {
        currentHighlightedAction?.ForceNormalState();
        currentHighlightedAction = null;
        UnRenderHand();
    }

    private void HandleEnemyClick(EnemyClass clickedEnemy)
    {
        if (selectedPlayer == null)
        {
            PopUpNotificationManager.Instance.DisplayWarning(PopupType.SelectPlayerFirst);
            return;
        }

        if (currentHighlightedAction == null)
        {
            PopUpNotificationManager.Instance.DisplayWarning(PopupType.SelectActionFirst);
            return;
        }

        if (currentHighlightedEnemyEntity == null)
        {
            currentHighlightedEnemyEntity = clickedEnemy;
            currentHighlightedEnemyEntity.Highlight();
        }
        else if (currentHighlightedEnemyEntity != clickedEnemy)
        {
            currentHighlightedEnemyEntity.DeHighlight();
            clickedEnemy.Highlight();
            currentHighlightedEnemyEntity = clickedEnemy;
        } else
        {
            currentHighlightedEnemyEntity.DeHighlight();
        }

        if (currentHighlightedEnemyEntity != null && currentHighlightedAction != null)
        {
            ProcessActionOnEnemy();
        }
    }

    private void ProcessActionOnEnemy()
    {
        currentHighlightedAction!.Target = currentHighlightedEnemyEntity;
        BattleQueue.BattleQueueInstance.AddAction(currentHighlightedAction);
        PlayerManuallyInsertedAction?.Invoke(currentHighlightedAction);
        selectedPlayer!.HandleUseCard(currentHighlightedAction);


        currentHighlightedEnemyEntity!.DeHighlight();
        currentHighlightedAction.ForceNormalState();
        currentHighlightedEnemyEntity = null;
        currentHighlightedAction = null;
    }

    public void OnActionClicked(ActionClass clicked)
    {
        if (CombatManager.Instance.GameState != GameState.SELECTION || PauseMenu.IsPaused) return;
        if (selectedPlayer != null)
        {
            if (clicked.IsPlayableByPlayer(out PopupType popupType) == false)
            {
                PopUpNotificationManager.Instance.DisplayWarning(popupType);
                return;
            }

            if (currentHighlightedAction == null)
            {
                clicked.ToggleSelected();
                currentHighlightedAction = clicked;
            }
            else if (currentHighlightedAction != clicked)
            {
                currentHighlightedAction.ForceNormalState();
                clicked.ToggleSelected();
                currentHighlightedAction = clicked;
            }
            else //Current Action is the same as clicked
            {
                currentHighlightedAction.ToggleUnSelected();
                currentHighlightedAction = null;
                if (currentHighlightedEnemyEntity != null)
                {
                    currentHighlightedEnemyEntity.DeHighlight();
                }
            }
        }
    }

    private void ResetSelection(GameState gameState)
    {
        if (gameState == GameState.FIGHTING)
        {
            //Untoggle card if it is still selected when entering fighting
            if (currentHighlightedAction != null)
            {
                currentHighlightedAction.ForceNormalState();
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
        RenderHand(player.Hand, player.Pool);
    }

    /*  Renders the cards in List<GameObject> hand to the screen, as children of the handContainer.
    *  Cards are filled in left to right.
    *  REQUIRES: Nothing
    *  MODIFIES: Nothing
    * 
    */
    private void RenderHand(List<GameObject> hand, List<GameObject> pool)
    {
        for (int i = 0; i < hand.Count; i++)
        {
            GameObject handItem = hand[i];
            ActionClass insertingAction = handItem.GetComponent<ActionClass>();
            insertingAction.ForceNormalState();
            handItem.transform.SetParent(handContainer.transform, false);
            handItem.transform.position = Vector3.zero;

            float distanceToLeft = (float)(handContainer.rect.width / 2 - (i * CARD_WIDTH));

            float y = handContainer.transform.position.y;
            float x = handContainer.transform.position.x;
            Vector3 v = new Vector3(x-distanceToLeft, y, -i);
            handItem.transform.position = v;
            handItem.transform.rotation = Quaternion.Euler(0, 0, -5);

            insertingAction.SetCanPlay(insertingAction.IsPlayableByPlayer(out _));
        }
        RenderText(hand);
        deckRemainderViewController.Notify(pool);
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

    private void UnRenderHand()
    {
        List<GameObject> hand = new List<GameObject>();
        foreach (Transform child in handContainer)
        {
            hand.Add(child.gameObject);
        }

        for (int i = 0; i < hand.Count; i++)
        {
            hand[i].transform.SetParent(deckContainer, false);
            hand[i].transform.localPosition = Vector3.zero;
        }
    }
    
}
