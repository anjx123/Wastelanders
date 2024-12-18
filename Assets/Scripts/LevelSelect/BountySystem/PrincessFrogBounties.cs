using BountySystem;
using System.Collections.Generic;

// PrincessFrogBounties are a collection of challenges. These are to be used in game for the player to select a group of challenges at the same time;
public class PrincessFrogBounties : IBounties
{
    public static readonly PrincessFrogBounties PRINCESS_FROG_CHALLENGE = new PrincessFrogBounties(
        contractSet: new HashSet<IContracts> 
        { 
            PrincessFrogContracts.EXTRA_HP, 
            PrincessFrogContracts.ADDITIONAL_ATTACK, 
            PrincessFrogContracts.AGGRESIVE_AI,
            PrincessFrogContracts.HIGHER_ENEMIES_CAP
        },
        challengeName: "Buffed Princess",
        flavourText: "The Princess is back, and she doesn’t intend to lose this time.",
        rewards: "Princess Frog Cards"
        );

    public static readonly PrincessFrogBounties QUEEN_CHALLENGE = new PrincessFrogBounties(
        contractSet: new HashSet<IContracts> { EnemySpawningContracts.QUEEN_BEETLE_SPAWN },
        challengeName: "Queen and Princess",
        flavourText: "Get ready for a royal bloodbath.",
        rewards: "Queen Beetle Cards"
        );

    public static readonly PrincessFrogBounties SLIME_CHALLENGE = new PrincessFrogBounties(
        contractSet: new HashSet<IContracts> { EnemySpawningContracts.SLIME_SPAWN },
        challengeName: "SlimeFest",
        flavourText: "Test your skills in this slimy situation!",
        rewards: "Slime Cards"
        );

    public static readonly PrincessFrogBounties FROG_CHALLENGE = new PrincessFrogBounties(
        contractSet: new HashSet<IContracts> { EnemySpawningContracts.FROG_SPAWN },
        challengeName: "FrogFest",
        flavourText: "These frogs have gone wild, so hop to it!",
        rewards: "Frog Cards"
        );

    public static readonly PrincessFrogBounties SOLO_JACKIE = new PrincessFrogBounties(
        contractSet: new HashSet<IContracts> { PlayerContracts.SOLO_JACKIE },
        challengeName: "Solo Mission",
        flavourText: "Take on the Princess Beetle alone.",
        rewards: "Beetle Cards"
        );

    public static readonly PrincessFrogBounties DECREASED_HAND_SIZE = new PrincessFrogBounties(
        contractSet: new HashSet<IContracts> { PlayerContracts.DECREASED_HAND_SIZE },
        challengeName: "Exhausting Mission",
        flavourText: "Fight the Princess Beetle with a restricting hand size.",
        rewards: "TBD"
        );

    // Update this every time you add a new bounty please, I don't want to implement reflection.
    public static IEnumerable<PrincessFrogBounties> Values
    {
        get
        {
            yield return PRINCESS_FROG_CHALLENGE;
            yield return QUEEN_CHALLENGE;
            yield return SLIME_CHALLENGE;
            yield return FROG_CHALLENGE;
            yield return SOLO_JACKIE;
            yield return DECREASED_HAND_SIZE;
        }
    }
    public HashSet<IContracts> ContractSet { get; }
    public string FlavourText { get; }
    public string BountyName { get; }
    public string Rewards { get; }

    public string SceneName => GameStateManager.PRINCESS_FROG_BOUNTY;

    public PrincessFrogBounties(HashSet<IContracts> contractSet, string challengeName, string flavourText, string rewards)
    {
        ContractSet = contractSet;
        FlavourText = flavourText;
        BountyName = challengeName;
        Rewards = rewards;
    }
}