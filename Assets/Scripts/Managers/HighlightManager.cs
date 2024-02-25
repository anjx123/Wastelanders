using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HighlightManager : MonoBehaviour // later all entity highlighter
{
    private List<PlayerClass> players;

#nullable enable
    private static EntityClass? currentHighlightedEntity = null;
    private static ActionClass? currentHighlightedAction = null;
    public static PlayerClass? selectedPlayer = null;

    private void Start()
    {
        CombatManager.OnGameStateChanged += ResetSelection;
        players = CombatManager.Instance.GetPlayers();
    }

    private void OnDestroy()
    {
        CombatManager.OnGameStateChanged -= ResetSelection;
    }

    static HighlightManager()
    {
        currentHighlightedEntity = null; // no entity highlighted
        currentHighlightedAction = null; // no action highlighted
    }

    public static void OnEntityClicked(EntityClass clicked)
    {
        if (CombatManager.Instance.GameState != GameState.SELECTION) return;
        bool isOutlined = false;

        Debug.Log(clicked.GetType());

        if (clicked is PlayerClass)
        {
            ((PlayerClass)clicked).PleaseRenderMyHand();
        }

        if (currentHighlightedAction == null) 
        {
            PopUpNotificationManager.Instance.DisplayWarning(PopupType.SelectEnemyFirst);
            // no call to PQueue. 

        } else if (currentHighlightedEntity == null)
        {
            currentHighlightedEntity = clicked;
            currentHighlightedEntity.Highlight();
            isOutlined = true;
            // no call to PQueue.

        }
        else if (currentHighlightedEntity != clicked)
        {
            currentHighlightedEntity.DeHighlight();
            clicked.Highlight();
            currentHighlightedEntity = clicked;
            isOutlined = true;
            // no call to PQueue. 
        }
        else
        {
            isOutlined = currentHighlightedEntity.Toggle();
            // no call to PQueue.
        }

        if (currentHighlightedEntity != null && currentHighlightedAction != null && isOutlined)
        {
            currentHighlightedAction.Target = currentHighlightedEntity;
            currentHighlightedAction.Origin = selectedPlayer;
            // ------------------------------------------
            // ActionClass action = new QuickDraw(); // not possible; must be added using AddComponent method. was irrelevenat in the first place 
            // BUT damn Alissa you circumvented the entire problem using Event Managers!
            bool wasAdded = BattleQueue.BattleQueueInstance.AddPlayerAction(currentHighlightedAction); // action class is abstract using a derivative TODO.

            
            // ------------------------------------------- 
            // this requires rectification: the logic is cogent but we want all of this AFTER confirmation.

            //currentHighlightedEntity.TakeDamage(currentHighlightedAction.getRolledDamage());
            //Debug.Log("attack: " + currentHighlightedAction.getName() + ", target: " + currentHighlightedEntity.Id + ", damage: " + currentHighlightedAction.getRolledDamage());

            currentHighlightedEntity.DeHighlight();
            currentHighlightedAction.DeHighlight();
            if (selectedPlayer != null && wasAdded)
            {
                selectedPlayer.HandleUseCard(currentHighlightedAction);
            } else
            {
                PopUpNotificationManager.Instance.DisplayWarning(PopupType.SameSpeed);
            }
            currentHighlightedEntity = null;
            currentHighlightedAction = null;   
        }
    }

    public static void OnActionClicked(ActionClass clicked)
    {
        if (CombatManager.Instance.GameState != GameState.SELECTION) return;
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
                if (currentHighlightedEntity != null) 
                {
                    currentHighlightedEntity.DeHighlight();
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
                if (currentHighlightedEntity != null)
                {
                    currentHighlightedEntity.DeHighlight();
                }

            }
            // temp implementation
            if (selectedPlayer == null)
            {
                return;
            } else
            {
                selectedPlayer.UnRenderHand();
            }
        } 
    }

    public static bool RenderHandIfAppropriate(PlayerClass player)
    {
        if (selectedPlayer == player)
        {
            player.PleaseRenderMyHand();
            return true;
        } else
        {
            return false;
        }
    }

    // destruction should automatically destory the object itself and therefore it's container.
}
