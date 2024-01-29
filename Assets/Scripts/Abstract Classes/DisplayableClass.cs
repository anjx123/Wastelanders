using System;

// This class is a parent class of both BattleQueueIcons and CombatCardUI. This is because both of them
// are "displayable" in the upper right window when clicked.
public abstract class DisplayableClass : SelectClass
{
    public ActionClass actionClass; // Current/last ActionClass that we are displaying; it is set by the enemy
    public bool targetHighlighted = false;

}

