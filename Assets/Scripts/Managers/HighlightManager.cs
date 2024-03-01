using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HighlightManager : MonoBehaviour // later all entity highlighter
{

#nullable enable
    private static EntityClass? currentHighlightedEnemyEntity = null;
    private static ActionClass? currentHighlightedAction = null;
    public static PlayerClass? selectedPlayer = null;

    private void Start()
    {
        CombatManager.OnGameStateChanged += ResetSelection;
    }

    private void OnDestroy()
    {
        CombatManager.OnGameStateChanged -= ResetSelection;
    }

    public static void OnEntityClicked(EntityClass clicked)
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
                selectedPlayer?.UnRenderHand();
            }
            selectedPlayer = (PlayerClass)clicked;
            ((PlayerClass)clicked).RenderHand();
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
            selectedPlayer?.UnRenderHand();
        } 
    }

    // This method is actually redundant since you would HAVE to see the updated deck if you have a player selected.
    public static bool RenderHandIfAppropriate(PlayerClass player)
    {
        if (selectedPlayer == player)
        {
            player.RenderHand();
            return true;
        } else
        {
            selectedPlayer?.UnRenderHand();
            player.RenderHand();
            return false;
        }
    }
}
