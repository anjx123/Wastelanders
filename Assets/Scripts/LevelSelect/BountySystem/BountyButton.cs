using BountySystem;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// A Specific Bounty Button Component
public class BountyButton : MonoBehaviour
{
    [SerializeField] private TMP_Text _textMeshPro;
    [SerializeField] private Image image;
#nullable enable
    private bool selected = false;
    
    // late init !!
    private IBounties? bounty = null;
    private ColorBlock activeColors;
    private ColorBlock inactiveColors;
    private const float INACTIVE_ALPHA = 0.5f;

    protected virtual void Awake()
    {
        // hmmmm uhh hmmm, prolly a better way to do this through unity and not just script
        activeColors = GetComponent<Button>().colors;
        inactiveColors = activeColors;
        var inactiveNormalColor = inactiveColors.normalColor;
        inactiveNormalColor.a = INACTIVE_ALPHA;
        inactiveColors.normalColor = inactiveNormalColor;
        inactiveColors.selectedColor = inactiveNormalColor;

        GetComponent<Button>().colors = selected ? activeColors : inactiveColors;
    }

    public void OnSelect() 
    {
        selected = !selected;
        BountyManager.Instance.ActiveBounty = bounty;
        GetComponent<Button>().colors = selected ? activeColors : inactiveColors;
    }

    public void Initialize(IBounties bounty)
    {
        this.bounty = bounty;
        Redraw();
    }

    private void Redraw()
    {
        _textMeshPro.text = bounty?.BountyName;
    }
}