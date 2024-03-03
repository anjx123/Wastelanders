using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeetleScout : BeetleMinions
{
    protected ActionClass excavateClone;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        MaxHealth = 7;
        Health = MaxHealth;
        myName = "Scout";
    }

    public override void AddAttack(List<PlayerClass> players)
    {
        if (players.Count == 0) return;
        pool[0].GetComponent<ActionClass>().Target = players[Random.Range(0, players.Count - 1)];
        BattleQueue.BattleQueueInstance.AddEnemyAction(pool[0].GetComponent<ActionClass>(), this);
        combatInfo.SetCombatSprite(pool[0].GetComponent<ActionClass>());
        combatInfo.GetComponentInChildren<CombatCardUI>().actionClass = pool[0].GetComponent<ActionClass>();

        ActionClass cloned = pool[1].GetComponent<ActionClass>();
        cloned.Target = players[Random.Range(0, players.Count - 1)];
        BattleQueue.BattleQueueInstance.AddEnemyAction(cloned, this);
        combatInfo.SetCombatSprite(cloned);
        combatInfo.GetComponentInChildren<CombatCardUI>().actionClass = cloned;
        
    }
}
