using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


# nullable enable
namespace BountySystem
{
    public interface IContracts
    {

        // Update this if a new class implements me please, I don't want to use reflection.
        public static IEnumerable<IEnumerable<IContracts>> Values
        {
            get
            {
                yield return PlayerContracts.Values;
            }
        }
    }

    // Organizing Player Challenges into its own set, might be better to just put all contracts in one place
    public class PlayerContracts : IContracts
    {
        public static readonly PlayerContracts SOLO_JACKIE = new PlayerContracts();

        public static IEnumerable<PlayerContracts> Values
        {
            get
            {
                yield return SOLO_JACKIE;
            }
        }
    }

    public interface IBounties
    {
        public HashSet<IContracts> BountySet { get; }
        public string FlavourText { get; }
        public string ChallengeName { get; }

        // Update this if a new class implements me please, I don't want to use reflection.
        public static IEnumerable<IEnumerable<IBounties>> Values
        {
            get
            {
                yield return PrincessFrogBounties.Values;
            }
        }

        public static void MapOnValues(Action<IBounties> action)
        {
            foreach (var bountySet in Values)
            {
                foreach (var bounty in bountySet)
                {
                    action.Invoke(bounty);
                }
            }
        }
    }
}
