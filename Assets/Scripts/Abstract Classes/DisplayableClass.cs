﻿using System;

// This class is a parent class of both BattleQueueIcons and CombatCardUI. This is because both of them
// are "displayable" in the upper right window when clicked.
public abstract class DisplayableClass : SelectClass
{
    public ActionClass actionClass; // Associated ActionClass that this class displays; it is set by the entity
    protected bool targetHighlighted = false;

    protected void ShowCard()
    {
        CombatCardDisplayManager.Instance.ShowCard(actionClass);
    }

    protected void HighlightTarget()
    {
        if (!targetHighlighted)
        {
            actionClass.Target.OnMouseEnter();
        }
        targetHighlighted = true;
    }

    protected void DeHighlightTarget()
    {
        if (targetHighlighted)
        {
            actionClass.Target.OnMouseExit();
        }
        targetHighlighted = false;
    }
}
