using UnityEngine;
using System;
using Systems.Persistence;
using System.Collections.Generic;
using System.Linq;
using BountySystem;
using LevelSelectInformation;

# nullable enable
// A class that persists the current bounty information during level selecting
public class BountyManager : PersistentSingleton<BountyManager>, IBind<BountyStateData>
{
    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
    private BountyStateData? ContractStateData; 
    public BountyInformation? SelectedBountyInformation { get; set; } = null;

    // All ActiveBounty should be contained within BountyInformation's Bounty Collection
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

// The serialized 
[System.Serializable]
public class BountyStateData : ISaveable
{
    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
    [field: SerializeField] public List<ChallengeCompletionState> BountyCompletionData { get; set; } = new();

    public void SetChallengeComplete(IBounties bounty)
    {
        ChallengeCompletionState? challengeCompletionState = BountyCompletionData.Find(data => data.BountyName == bounty.BountyName);
        
        if (challengeCompletionState == null) BountyCompletionData.Add(new(bounty.BountyName, true));
        else challengeCompletionState.Completed = true;
    }

    public BountyStateData()
    {
        Initialize();
    }

    private void Initialize()
    {
        BountyCompletionData.Clear();
        IBounties.MapOnValues(bounty => BountyCompletionData.Add(new ChallengeCompletionState(bounty.BountyName, false)));
    }

    [Serializable]
    public class ChallengeCompletionState
    {
        [field: SerializeField] public bool Completed { get; set; }
        [field: SerializeField] public string BountyName { get; }
        
        public ChallengeCompletionState(string bountyName, bool completed)
        {
            Completed = completed;
            BountyName = bountyName;
        }
    }
}
