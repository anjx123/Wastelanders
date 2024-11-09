using BountySystem;
using System.Collections.Generic;

// PrincessFrogBounties are a collection of challenges. These are to be used in game for the player to select a group of challenges at the same time;
public class PrincessFrogBounties : IBounties
{
    public static readonly PrincessFrogBounties PRINCESS_FROG_CHALLENGE = new PrincessFrogBounties(
        bountySet: new HashSet<IContracts> { },
        challengeName: "The Princess Frog Challenge",
        flavourText: "FlavourText"
        );

    public static readonly PrincessFrogBounties QUEEN_CHALLENGE = new PrincessFrogBounties(
        bountySet: new HashSet<IContracts> { },
        challengeName: "The Queen Challenge",
        flavourText: "FlavourText"
        );

    public static readonly PrincessFrogBounties SLIME_CHALLENGE = new PrincessFrogBounties(
        bountySet: new HashSet<IContracts> { },
        challengeName: "The Slime Challenge",
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
        }
    }
    public HashSet<IContracts> BountySet { get; }
    public string FlavourText { get; }
    public string ChallengeName { get; }

    public string SceneName => GameStateManager.PRINCESS_FROG_BOUNTY;

    public PrincessFrogBounties(HashSet<IContracts> bountySet, string challengeName, string flavourText)
    {
        BountySet = bountySet;
        FlavourText = flavourText;
        ChallengeName = challengeName;
    }
}