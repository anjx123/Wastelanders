using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ives : PlayerClass
{
    // @author: andrew
    // i copy pasted jackie's code. it may not work completely, this is just for testing purposes.
    public override void Start()
    {
        MaxHealth = 35;
        Health = MaxHealth;
        myName = "Ives";
        base.Start();
    }

    protected override void GrabDeck()
    {
        cardPrefabs = CombatManager.Instance.GetDeck(PlayerDatabase.PlayerName.IVES);
    }
}

