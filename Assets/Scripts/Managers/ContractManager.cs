using UnityEngine;
using System;
using Systems.Persistence;
using System.Collections.Generic;
using static Contracts;
using static Challenges;

/*
    If you are looking to modify/add contracts, your attention should mostly be in Contracts and Challenges
        Read the comments regarding the respective roles of each

    If you are looking to add new levels and their respective contracts and challenges, modify ContractStateData
        following the existing pattern, and also add the new contracts to the sanity checkers at the bottom
*/

// Contracts are individual/atomic modifiers. One contract should be responsible for one change and not a group of changes
public static class Contracts
{
    
    // These enums represent contracts that are available
    public enum FrogContracts
    {
        PrincessPlus1LowerBound,
        PrincessPlus10ResonateStacks,
        PrincessExtraBuff,
        PrincessExtraCardPerTurn
    }

    public enum BeetleContracts
    {
        PlusHealth
    }

    public enum QueenContracts
    {
        SoloJackie
    }

    // This is currently not being used, but is potential for future considerations
    // when other values related to contracts may be needed
    public struct ContractValues
    {
        public bool Completed;

        public ContractValues(bool completed = false)
        {
            Completed = completed;
        }
    }
}

// Challenges are a collection of contracts. These are to be used in game for the player to select a group of contracts
public static class Challenges
{
    // Enums represent the available challenges
    public enum FrogChallenges
    {
        QueenFrog
    }

    public enum BeetleChallenges
    {
        Tougher
    }

    public enum QueenChallenges
    {
        Soloist
    }

    // The map to collect all the contracts from the challenge
    public static readonly Dictionary<Enum, HashSet<Enum>> ChallengeContracts = new Dictionary<Enum, HashSet<Enum>>
    {
        { 
            FrogChallenges.QueenFrog, new HashSet<Enum>
            { 
                FrogContracts.PrincessExtraBuff,
                FrogContracts.PrincessExtraCardPerTurn,
                FrogContracts.PrincessPlus10ResonateStacks,
                FrogContracts.PrincessPlus1LowerBound 
            }
        },
        {
            BeetleChallenges.Tougher, new HashSet<Enum>
            {
                BeetleContracts.PlusHealth
            }
        },
        {
            QueenChallenges.Soloist, new HashSet<Enum>
            {
                QueenContracts.SoloJackie,
            }
        }
    };

    public static readonly Dictionary<Enum, string> ChallengeFlavourText = new Dictionary<Enum, string>
    {
        {
            FrogChallenges.QueenFrog, "uhhh uhhhh"
        },
        {
            BeetleChallenges.Tougher, "uhhh ur tougher"
        },
        {
            QueenChallenges.Soloist, "uhhh ur alone"
        }
    };
}

[Serializable] class ContractStateData : ISaveable
{
    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
    [field: SerializeField] public Dictionary<Enum, bool> ContractValues;
    [field: SerializeField] public Dictionary<Enum, bool> ChallengeValues;

    // Constructor when data isn't found
    public ContractStateData()
    {
        ContractValues = new Dictionary<Enum, bool>();

        foreach (FrogContracts contract in Enum.GetValues(typeof(FrogContracts)))
        {
            ContractValues[contract] = false;
        }
        foreach (BeetleContracts contract in Enum.GetValues(typeof(BeetleContracts)))
        {
            ContractValues[contract] = false;
        }
        foreach (QueenContracts contract in Enum.GetValues(typeof(QueenContracts)))
        {
            ContractValues[contract] = false;
        }

        ChallengeValues = new Dictionary<Enum, bool>();

        foreach (FrogChallenges contract in Enum.GetValues(typeof(FrogChallenges)))
        {
            ChallengeValues[contract] = false;
        }
        foreach (BeetleChallenges contract in Enum.GetValues(typeof(BeetleChallenges)))
        {
            ChallengeValues[contract] = false;
        }
        foreach (QueenChallenges contract in Enum.GetValues(typeof(QueenChallenges)))
        {
            ChallengeValues[contract] = false;
        }
    }
}

// ContractManager should be the only interface any other place interacts with contract related data
public class ContractManager : PersistentSingleton<ContractManager>, IBind<ContractStateData>
{
    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
    private ContractStateData ContractStateData;
    void IBind<ContractStateData>.Bind(ContractStateData data)
    {
        ContractStateData = data;
        Id = data.Id;
    }

    // Tracks currently selected contracts/challenge
    private Enum ActiveChallenge = null;
    private HashSet<Enum> ActiveContracts = new HashSet<Enum>();

    // Tracks active level for ContractSelect (should not be used elsewhere)
    public string SelectedLevel = "";

    // Clears active contracts, sets them as completed
    public void NotifyWin()
    {
        ContractStateData.ChallengeValues[ActiveChallenge] = true;

        foreach (Enum contract in ActiveContracts)
        {
            ContractStateData.ContractValues[contract] = true;
        }

        ActiveChallenge = null;
        ActiveContracts.Clear();
    }

    /*
        The current setup of getters and setters are so that:
            - The players pick the active challenges
            - Scene builders retrieve the active contracts associated with the contracts
            - Reward systems can retrieve specific completed contracts/challenges

        If any other getters and setters that are needed are simple to implement
        but may not be needed
    */

    // Set active challenge to null if "disabling" contracts
    // Returns its corresponding flavour text
    public string SetActiveChallenge(Enum challenge = null)
    {
        if (challenge == null) {
            ActiveChallenge = null;
            ActiveContracts.Clear();
            return "";
        }

        CheckChallenge(challenge);

        ActiveChallenge = challenge;
        ActiveContracts = ChallengeContracts[challenge];

        // DebugLogActiveContracts();

        return ChallengeFlavourText[challenge];
    }

    public bool GetActiveContract(Enum contract)
    {
        CheckContract(contract);

        return ActiveContracts.Contains(contract);
    }

    public bool GetCompletedChallenge(Enum challenge)
    {
        CheckChallenge(challenge);

        return ContractStateData.ChallengeValues[challenge];
    }

    public bool GetCompletedContract(Enum contract)
    {
        CheckContract(contract);

        return ContractStateData.ContractValues[contract];
    }

    // Sanity checkers
    private void CheckContract(Enum contract) 
    {
        if (contract.GetType() == typeof(FrogContracts) ||
            contract.GetType() == typeof(BeetleContracts) ||
            contract.GetType() == typeof(QueenContracts))
            return;
        else
            throw new ArgumentException("Non-contract enum provided!");
    }

    private void CheckChallenge(Enum contract) 
    {
        if (contract.GetType() == typeof(FrogChallenges) ||
            contract.GetType() == typeof(BeetleChallenges) ||
            contract.GetType() == typeof(QueenChallenges))
            return;
        else
            throw new ArgumentException("Non-challenge enum provided!");
    }

    private void DebugLogActiveContracts()
    {
        foreach (Enum contract in ActiveContracts)
        {
            Debug.Log(contract.GetType().Name + "." + Enum.GetName(contract.GetType(), contract));
        }
    }
}