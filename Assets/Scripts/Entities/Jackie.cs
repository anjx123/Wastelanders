using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jackie : PlayerClass
{

    // Start is called before the first frame update
    public override void Start()
    {
        myName = "Jackie";
        base.Start();
        MaxHealth = 30;
        Health = MaxHealth;

        cardPrefabs = CombatManager.Instance.GetDeck(PlayerDatabase.PlayerName.JACKIE);
        // deep copy deck into pool; this can't be abstracted because of dispatch rules; base.base.Start() requires these cards and these cards are introduced in the editor (added to the cardPrefabs field)
        for (int i = 0; i < cardPrefabs.Count; i++)
        {
            GameObject toAdd = Instantiate(cardPrefabs[i].gameObject);
            toAdd.GetComponent<ActionClass>().Origin = this;
            pool.Add(toAdd);
            toAdd.transform.position = new Vector3(-1000, -1000, 1);
        }
        // HighlightManager.selectedPlayer = this;      // MAGIC! Joke aside: irrelevant but keep for future debugging and obviation.
    }
}
