using TMPro;
using UnityEngine;

public class CombatCardDisplayManager : MonoBehaviour
{

    public static CombatCardDisplayManager Instance;
    public GameObject cardDisplay; // The card display object
    [SerializeField] private GameObject cardTemplatePrefab;
    private GameObject fullCardObject; // keep ref to object to destroy
    [SerializeField] private GameObject descriptionHolderPrefab;
    private GameObject descriptionHolder;

    [SerializeField] private Camera mainCamera;

#nullable enable
    public bool IsDisplaying { get; set; } = false;
    private ActionClass? currentUser;
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

    private void Start()
    {
        CombatManager.OnGameStateChanging += HideCard;

        fullCardObject = Instantiate(cardTemplatePrefab);
        fullCardObject.transform.SetParent(cardDisplay.transform, false);
        fullCardObject.transform.localPosition = new Vector3(0, 0, 0);
        fullCardObject.AddComponent<CardInfoDisable>();
        fullCardObject.SetActive(false);

        descriptionHolder = Instantiate(descriptionHolderPrefab);
        descriptionHolder.transform.SetParent(cardDisplay.transform, false);
        descriptionHolder.transform.localPosition = new Vector3(0, -2.5f, 0);
        descriptionHolder.GetComponentInChildren<MeshRenderer>().sortingLayerName = descriptionHolder.GetComponent<SpriteRenderer>().sortingLayerName;
        descriptionHolder.GetComponentInChildren<MeshRenderer>().sortingOrder = descriptionHolder.GetComponent<SpriteRenderer>().sortingOrder + 1;
        descriptionHolder.AddComponent<CardInfoDisable>();
        descriptionHolder.SetActive(false);
    }

    private void OnDestroy()
    {
        CombatManager.OnGameStateChanging -= HideCard;
        if (currentUser != null)
        {
            currentUser.TargetChanging -= DeHighlightTarget;
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
            fullCardObject.SetActive(false);
            descriptionHolder.SetActive(false);
            DeHighlightTarget(currentUser);
            currentUser = null;
        }
        else
        {
            fullCardObject.SetActive(true);
            descriptionHolder.SetActive(true);
            fullCardObject.GetComponentInChildren<CardUI>().RenderCard(a);
            descriptionHolder.GetComponentInChildren<TextMeshPro>().text = a.Description;
            if (currentUser != null)
            {
                DeHighlightTarget(currentUser);
            }
            currentUser = a;
            HighlightTarget(a);
        }
    }

    public void HideCardOnClick()
    {
        if (currentUser != null)
        {
            fullCardObject.SetActive(false);
            descriptionHolder.SetActive(false);
            DeHighlightTarget(currentUser);
            currentUser = null;
        }
    }


    // Hides the card by destroying the child. Don't need to pass any parameters in as the manager
    //  doesn't need to keep track of who calls this
    private void HideCard(GameState gameState)
    {
        if (gameState == GameState.FIGHTING)
        {
            if (fullCardObject != null)
            {
                fullCardObject.SetActive(false);
                descriptionHolder.SetActive(false);
            }
            if (currentUser != null)
            {
                DeHighlightTarget(currentUser);
                currentUser = null;
            }
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
        actionClass.TargetChanging += DeHighlightTarget;
    }

    // Deighlights the target of a
    private void DeHighlightTarget(ActionClass actionClass)
    {
        actionClass.TargetChanging -= DeHighlightTarget;
        if (targetHighlighted)
        {
            actionClass.Target.UnCrossHair();
        }
        targetHighlighted = false;
    }
}