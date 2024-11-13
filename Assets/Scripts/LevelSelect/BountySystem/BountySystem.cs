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
        public static readonly PlayerContracts SOLO_JACKIE = new();
        public static readonly PlayerContracts DECREASED_HAND_SIZE = new();
    }

    public class EnemySpawningContracts: IContracts
    {
        public static readonly EnemySpawningContracts QUEEN_BEETLE_SPAWN = new();
        public static readonly EnemySpawningContracts SLIME_SPAWN = new();
        public static readonly EnemySpawningContracts FROG_SPAWN = new();
    }


    public class PrincessFrogContracts : IContracts
    {
        public static readonly PrincessFrogContracts ADDITIONAL_ATTACK = new();
        public static readonly PrincessFrogContracts EXTRA_HP = new();
        public static readonly PrincessFrogContracts AGGRESIVE_AI = new();
        public static readonly PrincessFrogContracts HIGHER_ENEMIES_CAP = new();
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
