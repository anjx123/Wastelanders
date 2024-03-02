using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeetleScout : EnemyClass
{
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

        ActionClass clonedAttack = Instantiate(pool[0].GetComponent<ActionClass>());
        clonedAttack.Target = players[Random.Range(0, players.Count - 1)];
        BattleQueue.BattleQueueInstance.AddEnemyAction(clonedAttack, this);
        combatInfo.SetCombatSprite(clonedAttack);
        combatInfo.GetComponentInChildren<CombatCardUI>().actionClass = clonedAttack;

        pool.RemoveAt(0);
        if (pool.Count < 1)
        {
            Reshuffle();
        }
    }
}
