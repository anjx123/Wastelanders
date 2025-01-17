using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bounty Reward Icon Database", menuName = "Bounty Reward Icon Database")]
public class BountyAssetDatabase : ScriptableObject
{
    public PrincessFrogAssets PrincessFrogAssets;
}


[System.Serializable]
public class BountyAssets {
    public Sprite Sprite;
    public Sprite Background;
}