using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class HighlightManager // later all entity highlighter
{
    private static EntityClass currentHighlightedEntity
    ;
    private static ActionClass currentHighlightedAction;
    public static EntityClass player;

    static HighlightManager()
    {
        currentHighlightedEntity = null; // no entity highlighted
        currentHighlightedAction = null; // no action highlighted
    }

    public static void OnEntityClicked(EntityClass clicked)
    {
        bool isOutlined = false;

        if (currentHighlightedAction == null) 
        {
            Debug.Log("Select card first!");
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

        if (currentHighlightedEntity && currentHighlightedAction && isOutlined)
        {
            currentHighlightedAction.Target = currentHighlightedEntity;
            currentHighlightedAction.Origin = player;

            // ------------------------------------------
            // ActionClass action = new QuickDraw(); // not possible; must be added using AddComponent method. was irrelevenat in the first place 
            // BUT damn Alissa you circumvented the entire problem using Event Managers!
            BattleQueue.BattleQueueInstance.AddPlayerAction(currentHighlightedAction); // action class is abstract using a derivative TODO.
            BattleQueue.BattleQueueInstance.UpdateTest();
            
            // ------------------------------------------- 
            // this requires rectification: the logic is cogent but we want all of this AFTER confirmation.

            currentHighlightedEntity.TakeDamage(currentHighlightedAction.getDamage());
            Debug.Log("attack: " + currentHighlightedAction.getName() + ", target: " + currentHighlightedEntity.Id + ", damage: " + currentHighlightedAction.getDamage());
        }
    }

    public static void OnActionClicked(ActionClass clicked)
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
                if (currentHighlightedEntity) 
                {
                    currentHighlightedEntity.DeHighlight();
                }
                
                Debug.Log("Select card first!");
            }
        }
    }
}
