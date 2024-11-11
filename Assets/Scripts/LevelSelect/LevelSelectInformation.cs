using BountySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelSelectInformation
{
    public enum Level
    {
        Tutorial,
        FrogSlimeFight,
        BeetleFight,
        QueenFight,
        PrincessFrogFight,
        PrincessFrogBounty,
    }

    public interface ILevelSelectInformation
    {
        public float LevelID { get; set; }

        public void UponSelectedEvent();

        public static readonly Dictionary<Level, ILevelSelectInformation> LEVEL_INFORMATION = new()
        {
            { Level.Tutorial, StageInformation.TUTORIAL_STAGE },
            { Level.FrogSlimeFight, StageInformation.FROG_SLIME_STAGE },
            { Level.BeetleFight, StageInformation.BEETLE_STAGE },
            { Level.QueenFight, StageInformation.QUEEN_BEETLE_STAGE },
            { Level.PrincessFrogFight, StageInformation.PRINCESS_FROG_FIGHT },
            { Level.PrincessFrogBounty, BountyInformation.PRINCESS_FROG_BOUNTY },
        };
    }

    // Represents the information needed to load a specfic stage during level select
    public class StageInformation : ILevelSelectInformation
    {
        public static readonly StageInformation TUTORIAL_STAGE = new(sceneName: GameStateManager.TUTORIAL_FIGHT, levelId: 0);
        public static readonly StageInformation FROG_SLIME_STAGE = new(sceneName: GameStateManager.FROG_SLIME_FIGHT, levelId: 1f);
        public static readonly StageInformation BEETLE_STAGE = new(sceneName: GameStateManager.BEETLE_FIGHT, levelId: 2f);
        public static readonly StageInformation QUEEN_BEETLE_STAGE = new(sceneName: GameStateManager.PRE_QUEEN_FIGHT, levelId: 3f);
        public static readonly StageInformation PRINCESS_FROG_FIGHT = new(sceneName: GameStateManager.PRINCESS_FROG_BOUNTY, levelId: 4f);
        public string SceneName { get; set; }
        public float LevelID { get; set; }

        public delegate void StageInformationDelegate(string sceneName);
        public static event StageInformationDelegate StageInformationEvent;

        public StageInformation(string sceneName, float levelId)
        {
            SceneName = sceneName;
            LevelID = levelId;
        }

        public void UponSelectedEvent()
        {
            StageInformationEvent?.Invoke(SceneName);
        }
    }

    // Represents the information needed to load a specific bounty stage during level select
    public class BountyInformation : ILevelSelectInformation
    {
        public static readonly BountyInformation PRINCESS_FROG_BOUNTY = new(PrincessFrogBounties.Values, levelId: 4.5f);
        public IEnumerable<IBounties> BountyCollection { get; set; }
        //Bounties are encoded with .5f level id ending
        public float LevelID { get; set; }

        public delegate void BountyInformationDelegate(BountyInformation BountyType);
        public static event BountyInformationDelegate BountyInformationEvent;
        public BountyInformation(IEnumerable<IBounties> bounties, float levelId)
        {

            LevelID = levelId;
            BountyCollection = bounties;
        }

        public void UponSelectedEvent()
        {
            BountyInformationEvent?.Invoke(this);
        }
    }
}