using System;
using UnityEngine;
using UnityEngine.UI;

public class ContractButton : MonoBehaviour
{
    // the level, 0 being tutorial, 3 being queen
    public int level;
    public string contract;
    private bool selected;

    private ColorBlock activeColors;
    private ColorBlock inactiveColors;
    private const float INACTIVE_ALPHA = 0.5f;

    protected virtual void Awake()
    {
        switch (level) {
            case 0:
                Debug.LogError("No Tutorial Contracts!");
                break;
            case 1:
                selected = ContractManager.Instance.GetContract(StringToContract<FrogContracts>(contract));
                break;
            case 2:
                selected = ContractManager.Instance.GetContract(StringToContract<BeetleContracts>(contract));
                break;
            case 3:
                selected = ContractManager.Instance.GetContract(StringToContract<QueenContracts>(contract));
                break;
        }

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
        switch (level)
        {
            case 0:
                Debug.LogError("No Tutorial Contracts!");
                break;
            case 1:
                ToggleContract(StringToContract<FrogContracts>(contract));
                break;
            case 2:
                ToggleContract(StringToContract<BeetleContracts>(contract));
                break;
            case 3:
                ToggleContract(StringToContract<QueenContracts>(contract));
                break;
        }

        GetComponent<Button>().colors = selected ? activeColors : inactiveColors;
    }

    T StringToContract<T>(string contract) where T : Enum
    {
        return (T) Enum.Parse(typeof(T), contract);
    }

    void ToggleContract<T>(T contract) where T : Enum 
    {
        bool newVal = !ContractManager.Instance.GetContract(contract);
        ContractManager.Instance.SetSelected(contract, newVal);
    }

}