using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class ChargeUpPlayable : FrogAttacks, IPlayableFrogCard
{
    [SerializeField]
    private GameObject hurlPrefab;
    public override void Initialize()
    {
        base.Initialize();
        lowerBound = 1;
        upperBound = 3;
        Speed = 1;
        CostToAddToDeck = 2;

        description = "Block, if unstaggered, use 'Hurl' next turn";

        myName = "Charge Up";
        CardType = CardType.Defense;
    }



    public override void CardIsUnstaggered()
    {
        PlayerClass player = (PlayerClass)this.Origin;
        player.InjectCard(hurlPrefab);
        Origin.AttackAnimation("IsBlocking"); // :3

    }
}
