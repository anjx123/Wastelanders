using Cards.EnemyCards.FrogCards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlessCardPlayable : FrogAttacks
{
    public override void Initialize()
    {
        base.Initialize();

        myName = "Bless";
        description =
            "If not staggered: Gives all players a random positive buff, then this monster loses 1 Resonate.";

        lowerBound = upperBound = 1;
        Speed = 1;
        CardType = CardType.Defense;
        CostToAddToDeck = 2;
    }

    public override void CardIsUnstaggered()
    {
        var stacks = Origin.GetBuffStacks(Resonate.buffName);
        if (stacks < 1) return;

        Origin.AttackAnimation("IsBlocking");
        Origin.ReduceStacks(Resonate.buffName, 1);

        var players = Origin.transform.parent.GetComponentsInChildren<PlayerClass>();
        var buffs = new[] { Accuracy.buffName, Flow.buffName, Resonate.buffName };
        foreach (var player in players)
        {
            player.AddStacks(buffs[Random.Range(0, buffs.Length)], 1);
        }
    }
}
