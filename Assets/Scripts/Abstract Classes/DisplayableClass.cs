using System;
using UnityEngine;

// This class is a parent class of both BattleQueueIcons and CombatCardUI. This is because both of them
// are "displayable" in the upper right window when clicked.
public abstract class DisplayableClass : SelectClass
{
#nullable enable
    public ActionClass? ActionClass { get; protected set; }   
    protected bool targetHighlighted = false;
    private bool grewLarger;
    
    public static event Action<ActionClass>? OnShowCard;
    public static event Action<ActionClass>? OnHideCard;

    protected void ShowCard()
    {
        if (ActionClass != null)
        {
            OnShowCard?.Invoke(ActionClass);
        }
    }

    protected void HideCard()
    {
        if (ActionClass != null)
        {
            OnHideCard?.Invoke(ActionClass);
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

    public virtual void OnMouseEnter()
    {
        if (CombatManager.Instance.CanHighlight() && !grewLarger)
        {
            myTransform.localScale += new Vector3((float)0.05, (float)0.05, 0);
            grewLarger = true;
        }
    }

    public virtual void OnMouseExit()
    {
        if (grewLarger)
        {
            myTransform.localScale -= new Vector3((float)0.05, (float)0.05, 0);
            grewLarger = false;
        }
    }
}

