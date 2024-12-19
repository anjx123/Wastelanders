using BountySystem;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// A Specific Bounty Button Component
public class BountyButton : MonoBehaviour
{
    [SerializeField] private TMP_Text bountyTitle;
    [SerializeField] private SpriteRenderer bountyBackRenderer;
    [SerializeField] private SpriteRenderer rewardIconRenderer;
    [SerializeField] private SpriteRenderer selectedRenderer;
    [SerializeField] private Sprite completed;
    [SerializeField] private Sprite incomplete;

    [Range(0, 1)]
    [SerializeField] private float hoverAlpha; // transparency level of selectedSprite when mouse is over

    // late init !!
    private IBounties bounty;
#nullable enable
    private bool selected = false;
    private bool mouseOver = false;

    public delegate void BountyButtonDelegate(IBounties bounty);
    public static event BountyButtonDelegate? BountyOnSelectEvent;
    public static event BountyButtonDelegate? BountyOnHoverEvent; // Needed for updating popup board
    public static event BountyButtonDelegate? BountyOnHoverEndEvent;  // Needed for updating popup board

    public void SetRewardIcon(Sprite s)
    {
        rewardIconRenderer.sprite = s;
    }

    void OnEnable()
    {
        BountyOnSelectEvent += HandleOtherBountySelectedEvent;
    }

    void OnDisable()
    {
        BountyOnSelectEvent -= HandleOtherBountySelectedEvent;
    }

    void OnMouseOver()
    {
        mouseOver = true;
        if (Input.GetMouseButtonDown(0))
        {
            OnPress();
        }
    }

    void OnMouseEnter()
    {
        BountyOnHoverEvent?.Invoke(bounty);
        if (selected) return;
        selectedRenderer.gameObject.SetActive(true);
        UpdateSelectedAlpha(hoverAlpha);
    }

    void OnMouseExit()
    {
        BountyOnHoverEndEvent?.Invoke(bounty);
        mouseOver = false;
        if (selected) return;
        selectedRenderer.gameObject.SetActive(false);
        UpdateSelectedAlpha(1f);
    }

    private void HandleOtherBountySelectedEvent(IBounties bounty)
    {
        if (bounty != this.bounty) Deselected();
    }

    public void OnPress()
    {
        selected = !selected;
        if (selected)
        {
            Selected();
        }
        else
        {
            Deselected();
        }
    }

    private void Selected()
    {
        selected = true;
        BountyManager.Instance.ActiveBounty = bounty;
        BountyOnSelectEvent?.Invoke(bounty);
        selectedRenderer.gameObject.SetActive(true);
        UpdateSelectedAlpha(1f);
    }

    private void Deselected()
    {
        selected = false;
        if (BountyManager.Instance.ActiveBounty == bounty) BountyManager.Instance.ActiveBounty = null;
        if (!mouseOver) selectedRenderer.gameObject.SetActive(false);
        else UpdateSelectedAlpha(hoverAlpha);
    }

    public void Initialize(IBounties bounty)
    {
        this.bounty = bounty;
        Redraw();
    }

    private void Redraw()
    {
        bountyTitle.text = bounty?.BountyName;
        bountyBackRenderer.sprite = BountyManager.Instance.IsBountyCompleted(bounty) ? completed : incomplete;
    }

    private void UpdateSelectedAlpha(float alpha)
    {
        Color c = selectedRenderer.color;
        c.a = alpha;
        selectedRenderer.color = c;
    }
}