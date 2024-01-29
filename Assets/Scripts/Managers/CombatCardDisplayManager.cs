using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class CombatCardDisplayManager : MonoBehaviour
{

    public static CombatCardDisplayManager Instance;
    public GameObject cardDisplay; // The card display object

    public bool isDisplaying = false;
    public DisplayableClass currentUser;
    public SpriteRenderer rdr;
    private bool targetHighlighted = false;

    // Awake is called before Start.
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }

    //Given an ActionClass a to display, and the SOURCE of the call, shows the card in the display.
    //REQUIRES: This function should always be called from a DisplayableClass.When it is called,
    // source should be set to the caller.Below is an example call:


    // CombatCardDisplayManager.Instance.ShowCard(actionClass, this);

    //In other words, the second argument passed should always be "this".
    //MODIFIES: cardDisplay.sprite, currentUser, rdr, isDisplaying

    public void ShowCard(ActionClass a, DisplayableClass source)
    {
        rdr = cardDisplay.GetComponent<SpriteRenderer>();
        if (source == currentUser)
        {
            rdr.enabled = false;
            DeHighlightTarget(a);
        }
        else
        {
            rdr.enabled = true;
            rdr.sprite = a.fullCard;
            isDisplaying = true;
            if (currentUser != null)
            {
                DeHighlightTarget(currentUser.actionClass);
            }
            currentUser = source;
            HighlightTarget(a);
        }

    }

    // Hides the card by disabling the sprite renderer. Don't need to pass any parameters in as the manager
    //  doesn't need to keep track of who calls this
    public void HideCard()
    {
        isDisplaying = false;
        if (rdr != null)
        {
            rdr.enabled = false;
        }
        if (currentUser != null)
        {
            DeHighlightTarget(currentUser.actionClass);
            currentUser = null;
        }
    }

    // Highlights the target of a
    private void HighlightTarget(ActionClass a)
    {
        if (!targetHighlighted)
        {
            a.Target.OnMouseEnter();
        }
        targetHighlighted = true;
    }

    // Deighlights the target of a
    public void DeHighlightTarget(ActionClass a)
    {
        a.Target.OnMouseExit();
        targetHighlighted = false;
    }
}