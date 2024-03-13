using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ives : PlayerClass
{
    // Hi my name is Ives. Pwease implement me :_(
    private GameObject lhookCard;

    public void AddLeftHook()
    {
        if (lhookCard)
        {
            ActionClass addHook = lhookCard.GetComponent<ActionClass>();
            // BattleQueue.BattleQueueInstance.AddPlayerAction(addHook);
        }
    }
}
