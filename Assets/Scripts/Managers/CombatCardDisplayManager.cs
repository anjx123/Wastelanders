using TMPro;
using UnityEngine;

public class CombatCardDisplayManager : MonoBehaviour
{

    public static CombatCardDisplayManager Instance;
    public GameObject cardDisplay; // The card display object
    GameObject fullCardObject; // keep ref to object to destroy

    public bool IsDisplaying { get; set; } = false;
    private ActionClass currentUser;
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
        if (a == currentUser)
        {
            IsDisplaying = false;
            if (fullCardObject != null)
            {
                Destroy(fullCardObject);
                fullCardObject = null;
            }
            DeHighlightTarget(currentUser);
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
                fullCardObject.transform.Find("TextCanvas").transform.Find("Info Popup").transform.Find("Canvas").transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>().text = a.description;
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
    private void HighlightTarget(ActionClass actionClass)
    {
        if (!targetHighlighted)
        {
            actionClass.Target.CrossHair();
        }
        targetHighlighted = true;
        actionClass.targetChanged += DeHighlightTarget;
    }

    // Deighlights the target of a
    private void DeHighlightTarget(ActionClass actionClass)
    {
        actionClass.targetChanged -= DeHighlightTarget;
        if (targetHighlighted)
        {
            actionClass.Target.UnCrossHair();
        }
        targetHighlighted = false;
    }
}