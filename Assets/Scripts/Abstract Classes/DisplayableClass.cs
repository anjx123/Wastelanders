using System;

// This class is a parent class of both BattleQueueIcons and CombatCardUI. This is because both of them
// are "displayable" in the upper right window when clicked.
public abstract class DisplayableClass : SelectClass
{
#nullable enable
    public ActionClass? ActionClass { get; protected set; }   
    protected bool targetHighlighted = false;

    protected void ShowCard()
    {
        if (ActionClass != null)
        {
            CombatCardDisplayManager.Instance.ShowCard(ActionClass);
        }
    }

    protected void HighlightTarget()
    {
        if (!targetHighlighted)
        {
            ActionClass?.Target?.Highlight();
        }
        targetHighlighted = true;
    }

    protected void DeHighlightTarget()
    {
        if (targetHighlighted)
        {
            ActionClass?.Target?.DeHighlight();
        }
        targetHighlighted = false;
    }

}

