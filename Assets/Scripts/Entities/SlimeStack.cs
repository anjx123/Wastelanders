using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class SlimeStack : EnemyClass
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        MaxHealth = 15;
        Health = MaxHealth;
        myName = "Le Slime Stack";
    }

    /*  Contains: 1x Pound, 1x StackSmash
  * 
  */
    public override void AddAttack(List<PlayerClass> players)
    {
        pool[0].GetComponent<ActionClass>().Target = players[Random.Range(0, players.Count - 1)];
        BattleQueue.BattleQueueInstance.AddEnemyAction(pool[0].GetComponent<ActionClass>(), this);
        combatInfo.SetCombatSprite(pool[0].GetComponent<ActionClass>());
        pool.RemoveAt(0);
        if (pool.Count < 1)
        {
            Reshuffle();
        }
    }
}
