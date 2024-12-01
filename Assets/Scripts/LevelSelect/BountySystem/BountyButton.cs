using BountySystem;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// A Specific Bounty Button Component
public class BountyButton : MonoBehaviour
{
    [SerializeField] private TMP_Text _textMeshPro;
    [SerializeField] private Image checkMark;


    // late init !!
    private IBounties bounty;
#nullable enable
    private bool selected = false;
    private ColorBlock activeColors;
    private ColorBlock inactiveColors;
    private const float INACTIVE_ALPHA = 0.5f;

    public delegate void BountyButtonDelegate(IBounties bounty);
    public static event BountyButtonDelegate? BountyOnSelectEvent;

    protected virtual void Awake()
    {
        ApplyColouring();
    }

    private void ApplyColouring()
    {
        Button button = GetComponent<Button>();
        activeColors = button.colors;

        inactiveColors = activeColors;
        Color inactiveNormalColor = inactiveColors.normalColor;
        inactiveNormalColor.a = INACTIVE_ALPHA;
        inactiveColors.normalColor = inactiveNormalColor;
        inactiveColors.selectedColor = inactiveNormalColor;


        button.colors = selected ? activeColors : inactiveColors;
    }

    public void OnEnable()
    {
        BountyOnSelectEvent += HandleOtherBountySelectedEvent;
    }

    public void OnDisable()
    {
        BountyOnSelectEvent -= HandleOtherBountySelectedEvent;
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
        } else
        {
            Deselected();
        }
    }

    private void Selected()
    {
        selected = true;
        BountyManager.Instance.ActiveBounty = bounty;
        BountyOnSelectEvent?.Invoke(bounty);
        GetComponent<Button>().colors = activeColors;
    }

    private void Deselected()
    {
        selected = false;
        GetComponent<Button>().colors = inactiveColors;
    }

    public void Initialize(IBounties bounty)
    {
        this.bounty = bounty;
        Redraw();
    }

    private void Redraw()
    {
        _textMeshPro.text = bounty?.BountyName;
        checkMark.enabled = BountyManager.Instance.IsBountyCompleted(bounty);
    }
}