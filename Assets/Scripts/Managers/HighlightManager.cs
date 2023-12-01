using System.Collections;
using System.Collections.Generic;
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

        } else if (currentHighlightedEntity == null)
        {
            currentHighlightedEntity = clicked;
            currentHighlightedEntity.Highlight();
            isOutlined = true;
        }
        else if (currentHighlightedEntity != clicked)
        {
            currentHighlightedEntity.DeHighlight();
            clicked.Highlight();
            currentHighlightedEntity = clicked;
            isOutlined = true;
        }
        else
        {
            isOutlined = currentHighlightedEntity.Toggle();
        }

        if (currentHighlightedEntity && currentHighlightedAction && isOutlined)
        {
            currentHighlightedAction.Target = currentHighlightedEntity;
            currentHighlightedAction.Origin = player;
            currentHighlightedEntity.TakeDamage(currentHighlightedAction.getDamage());
            Debug.Log("attack: " + currentHighlightedAction.getName() + ", target: " + currentHighlightedEntity.Id + ", damage: " + currentHighlightedAction.getDamage());
            currentHighlightedEntity.DeHighlight();
            currentHighlightedAction.DeHighlight();
            currentHighlightedEntity = null;
            currentHighlightedAction = null;
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
