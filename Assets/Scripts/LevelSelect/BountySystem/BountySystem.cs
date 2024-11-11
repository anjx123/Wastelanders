using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


# nullable enable
namespace BountySystem
{
    public interface IContracts { }

    // Organizing Player Challenges into its own set, although might be better to just put all contracts in one place
    public class PlayerContracts : IContracts
    {
        public static readonly PlayerContracts SOLO_JACKIE = new PlayerContracts();


        // Update this every time you add a new contract please, I don't want to implement reflection.
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
        public string BountyName { get; }
        public string SceneName { get; }

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
