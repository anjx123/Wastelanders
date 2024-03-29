using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ives : PlayerClass
{
    // @author: andrew
    // i copy pasted jackie's code. it may not work completely, this is just for testing purposes.
    public override void Start()
    {
        base.Start();
        MaxHealth = 30;
        Health = MaxHealth;
        myName = "Ives";

        cardPrefabs = CombatManager.Instance.GetDeck(PlayerDatabase.PlayerName.IVES);
        // deep copy deck into pool; this can't be abstracted because of dispatch rules; base.base.Start() requires these cards and these cards are introduced in the editor (added to the cardPrefabs field)
        for (int i = 0; i < cardPrefabs.Count; i++)
        {
            GameObject toAdd = Instantiate(cardPrefabs[i].gameObject);
            toAdd.GetComponent<ActionClass>().Origin = this;
            pool.Add(toAdd);
            toAdd.transform.position = new Vector3(-1000, -1000, 1);
        }
   
    }
}

