using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ives : PlayerClass
{
    // Hi my name is Ives. Pwease implement me :_(

    // 
    public override void Start()
    {
        base.Start();
        MaxHealth = 30;
        Health = MaxHealth;
        myName = "Ives";

        // deep copy deck into pool; this can't be abstracted because of dispatch rules; base.base.Start() requires these cards and these cards are introduced in the editor (added to the cardPrefabs field)
        for (int i = 0; i < cardPrefabs.Count; i++)
        {
            GameObject toAdd = Instantiate(cardPrefabs[i]);
            toAdd.GetComponent<ActionClass>().Origin = this;
            pool.Add(toAdd);
            toAdd.transform.position = new Vector3(-1000, -1000, 1);
        }
   
    }
}

