using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HighlightManager // later all entity highlighter
{
    private static EntityClass currentHighlightedEntity
    ;
    private static ActionClass currentHighlightedAction;

    static HighlightManager()
    {
        currentHighlightedEntity = null; // no entity highlighted
        currentHighlightedAction = null; // no action highlighted
    }

    public static void OnEntityClicked(EntityClass clicked)
    {
        if (currentHighlightedEntity == null)
        {
            currentHighlightedEntity
             = clicked;
            currentHighlightedEntity
            .Highlight();
        }
        else if (currentHighlightedEntity != clicked)
        {
            currentHighlightedEntity
            .DeHighlight();
            clicked.Highlight();
            currentHighlightedEntity
             = clicked;
        }
        else
        {
            currentHighlightedEntity
            .Toggle();
        }
    }

    public static void OnActionClicked(ActionClass clicked)
    {
        if (currentHighlightedAction == null)
        {
            currentHighlightedAction
             = clicked;
            currentHighlightedAction
            .Highlight();
        }
        else if (currentHighlightedAction != clicked)
        {
            currentHighlightedAction
            .DeHighlight();
            clicked.Highlight();
            currentHighlightedAction = clicked;
        }
        else
        {
            currentHighlightedAction.Toggle();
        }
    }
}
