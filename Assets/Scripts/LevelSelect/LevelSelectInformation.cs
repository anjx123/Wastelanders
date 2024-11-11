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

        public void OpenScene();

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

        public void OpenScene()
        {
            StageInformationEvent?.Invoke(SceneName);
        }
    }

    public class BountyInformation : ILevelSelectInformation
    {
        public static readonly BountyInformation PRINCESS_FROG_BOUNTY = new(typeof(PrincessFrogBounties), levelId: 4.5f);
        public Type BountyType { get; set; }
        //Bounties are encoded with .5f 
        public float LevelID { get; set; }

        public delegate void BountyInformationDelegate(Type BountyType);
        public static event BountyInformationDelegate BountyInformationEvent;
        public BountyInformation(Type type, float levelId)
        {
            if (!typeof(IBounties).IsAssignableFrom(type)) throw new ArgumentException("Type must implement IBounties interface.", nameof(type));

            LevelID = levelId;
            BountyType = type;
        }

        public void OpenScene()
        {
            BountyInformationEvent?.Invoke(BountyType);
        }
    }
}