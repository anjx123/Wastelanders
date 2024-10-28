using UnityEngine;
using System;
using Systems.Persistence;
using System.Collections.Generic;

public enum FrogContracts
{
    ExtraSlime
}

public enum BeetleContracts
{
    PlusHealth = -1 // example of contract risk assessment evaluation
}

public enum QueenContracts
{
    AloneJackie,
    EvolvedQueen
}

public struct ContractValues {
    public bool Selected;
    public bool Completed; // potentially for future

    public ContractValues(bool selected = false, bool completed = false) {
        Selected = selected;
        Completed = completed;
    }
}

[System.Serializable]
class ContractStateData : ISaveable
{

    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
    [field: SerializeField] private Dictionary<FrogContracts, ContractValues> FrogContractValues;
    [field: SerializeField] private Dictionary<BeetleContracts, ContractValues> BeetleContractValues;
    [field: SerializeField] private Dictionary<QueenContracts, ContractValues> QueenContractValues;

    public ContractStateData()
    {
        FrogContractValues = new Dictionary<FrogContracts, ContractValues>();
        BeetleContractValues = new Dictionary<BeetleContracts, ContractValues>();
        QueenContractValues = new Dictionary<QueenContracts, ContractValues>();

        foreach (FrogContracts contract in Enum.GetValues(typeof(FrogContracts)))
        {
            FrogContractValues[contract] = new ContractValues();
        }

        foreach (BeetleContracts contract in Enum.GetValues(typeof(BeetleContracts)))
        {
            BeetleContractValues[contract] = new ContractValues();
        }

        foreach (QueenContracts contract in Enum.GetValues(typeof(QueenContracts)))
        {
            QueenContractValues[contract] = new ContractValues();
        }
    }

    public bool GetContract(FrogContracts contract)
    {
        return FrogContractValues[contract].Selected;
    }
    
    public bool GetContract(BeetleContracts contract)
    {
        return BeetleContractValues[contract].Selected;
    }
    
    public bool GetContract(QueenContracts contract)
    {
        return QueenContractValues[contract].Selected;
    }

    public void SetSelected(FrogContracts contract, bool selected)
    {
        ContractValues vals = FrogContractValues[contract];
        vals.Selected = selected;
        FrogContractValues[contract] = vals;
    }

    public void SetSelected(BeetleContracts contract, bool selected)
    {
        ContractValues vals = BeetleContractValues[contract];
        vals.Selected = selected;
        BeetleContractValues[contract] = vals;
    }

    public void SetSelected(QueenContracts contract, bool selected)
    {
        ContractValues vals = QueenContractValues[contract];
        vals.Selected = selected;
        QueenContractValues[contract] = vals;
    }
}

public class ContractManager : PersistentSingleton<ContractManager>, IBind<ContractStateData>
{
    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
    public string selectedLevel; // used to indicate which level is currently active, can be used to check whether contracts are used upon entering level
    
    private ContractStateData contractStateData;

    void IBind<ContractStateData>.Bind(ContractStateData data)
    {
        contractStateData = data;
        Id = data.Id;
    }

    private void checkData() {
        if (contractStateData == null)
        {
            // for future, consider loading saved version
            contractStateData = new ContractStateData();
        }
    }

    public bool GetContract<T>(T contract) where T : Enum
    {
        checkData();

        // scuffed switch, temp solution
        if (typeof(T) == typeof(FrogContracts))
            return contractStateData.GetContract((FrogContracts)(object) contract);
        else if (typeof(T) == typeof(BeetleContracts))
            return contractStateData.GetContract((BeetleContracts)(object) contract);
        else if (typeof(T) == typeof(QueenContracts))
            return contractStateData.GetContract((QueenContracts)(object) contract);
        else
            throw new ArgumentException("Invalid contract enum provided");
    }

    public void SetSelected<T>(T contract, bool selected) where T : Enum
    {
        checkData();

        if (typeof(T) == typeof(FrogContracts))
            contractStateData.SetSelected((FrogContracts)(object) contract, selected);
        else if (typeof(T) == typeof(BeetleContracts))
            contractStateData.SetSelected((BeetleContracts)(object) contract, selected);
        else if (typeof(T) == typeof(QueenContracts))
            contractStateData.SetSelected((QueenContracts)(object) contract, selected);
        else
            throw new ArgumentException("Invalid contract enum provided");
    }
}