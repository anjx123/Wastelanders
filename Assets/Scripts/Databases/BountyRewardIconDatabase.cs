using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bounty Reward Icon Database", menuName = "Bounty Reward Icon Database")]
public class BountyRewardIconDatabase : ScriptableObject
{
    [System.Serializable]
    public class BountyNameIconPair
    {
        [field: SerializeField] public string BountyName { get; set; }
        [field: SerializeField] public Sprite Icon { get; set; }
    }

    [SerializeField] public List<BountyNameIconPair> pairs;
}