using System;
public abstract class DisplayableClass : SelectClass
{
    public ActionClass actionClass; // Current/last ActionClass that we are displaying; it is set by the enemy
    public bool targetHighlighted = false;

}

