using UnityEngine;
using UnityEngine.UI;

// Originally was used to toggle specific contracts, repurposed to toggle specific challenges
public class ContractButton : MonoBehaviour
{
    // This should be ButtonPanel or whatever parent holds all the challenge button grids and the script ContractSelect.cs
    public GameObject contractSelect;

    // The level 0-based index, 0 being tutorial, 3 being queen
    // Temp solution to distinguish and enable button layouts
    public int level;
    public string contract;
    
    private bool selected;
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
        contractSelect.GetComponent<ContractSelect>().OnSelect(this, !selected);
    }

    public void setSelected(bool selected) {
        this.selected = selected;

        GetComponent<Button>().colors = selected ? activeColors : inactiveColors;
    }

}