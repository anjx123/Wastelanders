using UnityEngine;
using System;

public enum FrogContracts {
    uhhuhhhhh
}

public enum BeetleContracts {
    uhhuhhhhh
}

public enum QueenContracts {
    uhhuhhhhh
}

public class ContractManager : PersistentSingleton<ContractManager>
{
    public bool GetContract<T>(T contract) where T : Enum
    {
        // scuffed switch
        if (typeof(T) == typeof(FrogContracts))
            return GetFrogContract((FrogContracts)(object) contract);
        else if (typeof(T) == typeof(BeetleContracts))
            return GetBeetleContract((BeetleContracts)(object) contract);
        else if (typeof(T) == typeof(QueenContracts))
            return GetQueenContract((QueenContracts)(object) contract);
        else
            throw new ArgumentException("Invalid contract enum provided");
    }

    private bool GetFrogContract(FrogContracts contract) {
        return false;
    }
    
    private bool GetBeetleContract(BeetleContracts contract) {
        return false;
    }

    private bool GetQueenContract(QueenContracts contract) {
        return false;
    }
}