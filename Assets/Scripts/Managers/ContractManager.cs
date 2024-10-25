using UnityEngine;
using System;
using Systems.Persistence;
using System.Collections.Generic;

public enum FrogContracts
{
    PlaceholderFrogContract
}

public enum BeetleContracts
{
    PlaceholderBeetleContract
}

public enum QueenContracts
{
    PlaceholderQueenContract
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
    [field: SerializeField] public Dictionary<FrogContracts, ContractValues> FrogContractValues;
    [field: SerializeField] public Dictionary<BeetleContracts, ContractValues> BeetleContractValues;
    [field: SerializeField] public Dictionary<QueenContracts, ContractValues> QueenContractValues;

    public ContractStateData()
    {
        FrogContractValues = new Dictionary<FrogContracts, ContractValues>();
        BeetleContractValues = new Dictionary<BeetleContracts, ContractValues>();
        QueenContractValues = new Dictionary<QueenContracts, ContractValues>();

        foreach (FrogContracts contract in Enum.GetValues(typeof(FrogContracts)))
        {
            FrogContractValues[contract] = new ContractValues { Selected = false, Completed = false };
        }

        foreach (BeetleContracts contract in Enum.GetValues(typeof(BeetleContracts)))
        {
            BeetleContractValues[contract] = new ContractValues { Selected = false, Completed = false };
        }

        foreach (QueenContracts contract in Enum.GetValues(typeof(QueenContracts)))
        {
            QueenContractValues[contract] = new ContractValues { Selected = false, Completed = false };
        }
    }
}

public class ContractManager : PersistentSingleton<ContractManager>, IBind<ContractStateData>
{
    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
    private ContractStateData contractStateData;

    public bool GetContract<T>(T contract) where T : Enum
    {
        if (contractStateData == null)
        {
            // for future, consider loading saved version
            contractStateData = new ContractStateData();
        }

        // scuffed switch, temp solution
        if (typeof(T) == typeof(FrogContracts))
            return GetContract((FrogContracts)(object) contract);
        else if (typeof(T) == typeof(BeetleContracts))
            return GetContract((BeetleContracts)(object) contract);
        else if (typeof(T) == typeof(QueenContracts))
            return GetContract((QueenContracts)(object) contract);
        else
            throw new ArgumentException("Invalid contract enum provided");
    }

    private bool GetContract(FrogContracts contract)
    {
        return contractStateData.FrogContractValues[contract].Selected;
    }

    private bool GetContract(BeetleContracts contract)
    {
        return contractStateData.BeetleContractValues[contract].Selected;
    }

    private bool GetContract(QueenContracts contract)
    {
        return contractStateData.QueenContractValues[contract].Selected;
    }

    public void SetContract<T>(T contract, ContractValues values) {
        if (contractStateData == null)
        {
            // for future, consider loading saved version
            contractStateData = new ContractStateData();
        }
        
        if (typeof(T) == typeof(FrogContracts))
            SetContract((FrogContracts)(object) contract, values);
        else if (typeof(T) == typeof(BeetleContracts))
            SetContract((BeetleContracts)(object) contract, values);
        else if (typeof(T) == typeof(QueenContracts))
            SetContract((QueenContracts)(object) contract, values);
        else
            throw new ArgumentException("Invalid contract enum provided");
    }

    private void SetContract(FrogContracts contract, ContractValues values)
    {
        contractStateData.FrogContractValues[contract] = values;
    }

    private void SetContract(BeetleContracts contract, ContractValues values)
    {
        contractStateData.BeetleContractValues[contract] = values;
    }

    private void SetContract(QueenContracts contract, ContractValues values)
    {
        contractStateData.QueenContractValues[contract] = values;
    }

    void IBind<ContractStateData>.Bind(ContractStateData data)
    {
        contractStateData = data;
        Id = data.Id;
    }
}