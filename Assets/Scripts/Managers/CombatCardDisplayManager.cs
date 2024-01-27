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
            if (currentUser != null)
            {
                DeHighlightTarget(currentUser.actionClass);
            }
            currentUser = source;
            HighlightTarget(a);
        }
        
    }

    private void HighlightTarget(ActionClass a)
    {
        if (!targetHighlighted)
        {
            a.Target.OnMouseEnter();
        }
        targetHighlighted = true;
    }

    public void DeHighlightTarget(ActionClass a)
    {
        a.Target.OnMouseExit();
        targetHighlighted = false;
    }
}