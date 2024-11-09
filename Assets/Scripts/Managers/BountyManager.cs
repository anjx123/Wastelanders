using UnityEngine;
using System;
using Systems.Persistence;
using System.Collections.Generic;
using System.Linq;
using BountySystem;

# nullable enable
// A class that persists the current bounty information of the player
public class BountyManager : PersistentSingleton<BountyManager>, IBind<BountyStateData>
{
    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
    private BountyStateData? ContractStateData;

    //The literal type name of the concrete IBounties class that is selected. (You can't specify types in the Unity editor, so we use the class name instead) 
    public string? SelectedBountyTypeName { get; set; } = null;

    // All ActiveBounty should be of actual type SelectedBountyTypeName 
    public IBounties? ActiveBounty { get; set; } = null;


    public void NotifyWin()
    {
        if (ActiveBounty != null) ContractStateData?.SetChallengeComplete(ActiveBounty);
        ActiveBounty = null;
    }
    void IBind<BountyStateData>.Bind(BountyStateData data)
    {
        ContractStateData = data;
        Id = data.Id;
    }
}

[System.Serializable]
public class BountyStateData : ISaveable
{
    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
    [field: SerializeField] public List<ChallengeCompletionState> BountyCompletionData { get; set; } = new();

    public void SetChallengeComplete(IBounties contract)
    {
        var challengeCompletionState = BountyCompletionData.Find(data => data.ContractName == contract.ChallengeName);
        challengeCompletionState.Completed = true;
    }

    public BountyStateData()
    {
        Initialize();
    }

    private void Initialize()
    {
        BountyCompletionData.Clear();
        IBounties.MapOnValues(bounty => BountyCompletionData.Add(new ChallengeCompletionState(bounty.ChallengeName, false)));
    }

    [Serializable]
    public class ChallengeCompletionState
    {
        [field: SerializeField] public bool Completed { get; set; }
        [field: SerializeField] public string ContractName { get; }
        
        public ChallengeCompletionState(string contractName, bool completed)
        {
            Completed = completed;
            ContractName = contractName;
        }
    }
}
