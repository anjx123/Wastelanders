using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using TMPro;

public class CombatCardDisplayManager : MonoBehaviour
{

    public static CombatCardDisplayManager Instance;
    public GameObject cardDisplay; // The card display object
    GameObject fullCardObject; // keep ref to object to destroy

    public bool IsDisplaying { get; set; } = false;
    private ActionClass currentUser;
    private SpriteRenderer rdr;
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
    // source should be set to the caller. Below is an example call:


    // CombatCardDisplayManager.Instance.ShowCard(actionClass);

    //MODIFIES: cardDisplay.sprite, currentUser, rdr, isDisplaying

    public void ShowCard(ActionClass a)
    {
        rdr = cardDisplay.GetComponent<SpriteRenderer>();
        if (a == currentUser)
        {
            IsDisplaying = false;
            if (fullCardObject != null)
            {
                Destroy(fullCardObject);
                fullCardObject = null;
            }
            currentUser = null;
        }
        else
        {
            if (fullCardObject != null)
            {
                Destroy(fullCardObject);
            }
            fullCardObject = Instantiate(a.fullCardObjectPrefab);
            if (fullCardObject != null)
            {
                fullCardObject.transform.position = new Vector3(0, 0, 0);
                fullCardObject.transform.SetParent(cardDisplay.transform, false);
                UpdateText(a);
            }

            IsDisplaying = true;
            if (currentUser != null)
            {
                DeHighlightTarget(currentUser);
            }
            currentUser = a;
            HighlightTarget(a);
        }
    }

    private void UpdateText(ActionClass a)
    {
        // TODO: fix bug with not updating text properly? It works first time, but if you are swapping from another card it breaks.
        // lower priority for now since we don't change enemy card stats yet, but it will come eventually
        GameObject textContainer = cardDisplay.transform.GetChild(0).Find("TextCanvas").gameObject;
        textContainer.transform.Find("LowerBoundText").gameObject.GetComponent<TextMeshProUGUI>().text = a.GetCard().rollFloor.ToString();
        // more text updates...
    }

    // Hides the card by destroying the child. Don't need to pass any parameters in as the manager
    //  doesn't need to keep track of who calls this
    public void HideCard()
    {
        IsDisplaying = false;
        if (fullCardObject != null)
        {
            Destroy(fullCardObject);
        }
        if (currentUser != null)
        {
            DeHighlightTarget(currentUser);
            currentUser = null;
        }
    }

    // Highlights the target of a
    private void HighlightTarget(ActionClass a)
    {
        if (!targetHighlighted)
        {
            a.Target.CrossHair();
        }
        targetHighlighted = true;
    }

    // Deighlights the target of a
    private void DeHighlightTarget(ActionClass a)
    {
        if (targetHighlighted)
        {
            a.Target.UnCrossHair();
        }
        targetHighlighted = false;
    }
}