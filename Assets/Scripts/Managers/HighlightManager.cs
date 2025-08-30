using System;
using System.Collections.Generic;
using System.Linq;
using UI_Toolkit;
using UnityEngine;

public class HighlightManager : MonoBehaviour // later all entity highlighter
{

    public static HighlightManager Instance { get; private set; }
#nullable enable
    public EntityClass? currentHighlightedEnemyEntity = null;
    public ActionClass? currentHighlightedAction = null;
    public PlayerClass? selectedPlayer = null;

    public delegate void HighlightEventDelegate(EntityClass e);
    public event HighlightEventDelegate? EntityClicked;
    public delegate void ActionAddedDelegate(ActionClass card);
    public event ActionAddedDelegate? PlayerManuallyInsertedAction;

    public static event Action<List<ActionClass>>? OnUpdateHand;

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
        if (CombatManager.Instance.GameState != GameState.SELECTION || PauseMenuV2.IsPaused) return;
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
        if (forcedPlayer != null) RenderHand(forcedPlayer.Hand);
    }
    private void HandlePlayerClick(PlayerClass clickedPlayer)
    {
        if (clickedPlayer != selectedPlayer)
        {
            ResetCurrentHighlightedAction();
            selectedPlayer = clickedPlayer;
            RenderHand(clickedPlayer.Hand);
        }
    }

    private void ResetCurrentHighlightedAction()
    {
        currentHighlightedAction?.ForceNormalState();
        currentHighlightedAction = null;
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
        if (CombatManager.Instance.GameState != GameState.SELECTION || PauseMenuV2.IsPaused) return;
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
        } 
    }

    // Note that there should only be one instance per round in which selectedPlayer is null, hence the non-assertion. (initial player selection) 
    // Auto shifts to relevant player
    public void RenderHandIfAppropriate(PlayerClass player)
    {
        if (player == null)
        {
            throw new Exception("This method was called from an invalid location or there is a logic conundrum in OnEntityClicked");
        }

        selectedPlayer = player; 
        RenderHand(player.Hand);
    }

    private static void RenderHand(List<GameObject> hand) => OnUpdateHand?.Invoke(hand.Select(go => go.GetComponent<ActionClass>()).Where(ac => ac).ToList());
}