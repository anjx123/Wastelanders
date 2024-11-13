using BountySystem;
using System.Collections.Generic;

// PrincessFrogBounties are a collection of challenges. These are to be used in game for the player to select a group of challenges at the same time;
public class PrincessFrogBounties : IBounties
{
    public static readonly PrincessFrogBounties PRINCESS_FROG_CHALLENGE = new PrincessFrogBounties(
        bountySet: new HashSet<IContracts> 
        { 
            PrincessFrogContracts.EXTRA_HP, 
            PrincessFrogContracts.ADDITIONAL_ATTACK, 
            PrincessFrogContracts.AGGRESIVE_AI,
            PrincessFrogContracts.HIGHER_ENEMIES_CAP
        },
        challengeName: "The Princess Frog Challenge",
        flavourText: "FlavourText"
        );

    public static readonly PrincessFrogBounties QUEEN_CHALLENGE = new PrincessFrogBounties(
        bountySet: new HashSet<IContracts> { EnemySpawningContracts.QUEEN_BEETLE_SPAWN },
        challengeName: "The Queen Challenge",
        flavourText: "FlavourText"
        );

    public static readonly PrincessFrogBounties SLIME_CHALLENGE = new PrincessFrogBounties(
        bountySet: new HashSet<IContracts> { EnemySpawningContracts.SLIME_SPAWN },
        challengeName: "The Slime Challenge",
        flavourText: "FlavourText"
        );

    public static readonly PrincessFrogBounties FROG_CHALLENGE = new PrincessFrogBounties(
        bountySet: new HashSet<IContracts> { EnemySpawningContracts.FROG_SPAWN },
        challengeName: "The Frog Challenge",
        flavourText: "FlavourText"
        );

    public static readonly PrincessFrogBounties SOLO_JACKIE = new PrincessFrogBounties(
        bountySet: new HashSet<IContracts> { PlayerContracts.SOLO_JACKIE },
        challengeName: "The Solo Challenge",
        flavourText: "FlavourText"
        );

    public static readonly PrincessFrogBounties DECREASED_HAND_SIZE = new PrincessFrogBounties(
        bountySet: new HashSet<IContracts> { PlayerContracts.DECREASED_HAND_SIZE },
        challengeName: "The Exhausted Challenge",
        flavourText: "FlavourText"
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
    public HashSet<IContracts> BountySet { get; }
    public string FlavourText { get; }
    public string BountyName { get; }

    public string SceneName => GameStateManager.PRINCESS_FROG_BOUNTY;

    public PrincessFrogBounties(HashSet<IContracts> bountySet, string challengeName, string flavourText)
    {
        BountySet = bountySet;
        FlavourText = flavourText;
        BountyName = challengeName;
    }
}