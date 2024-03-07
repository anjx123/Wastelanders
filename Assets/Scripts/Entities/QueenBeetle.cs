using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class QueenBeetle : EnemyClass
{

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        MaxHealth = 30;
        Health = MaxHealth;
        myName = "The Queen";

        Beetle.OnGainBuffs += HandleGainedBuffs;
    }

    // event handler for Beetle.OnGainBuffs. This is called whenever a beetle tries to
    // add a buff.
    // Adds the stacks that were directed to the beetle to the Queen instead.
    private void HandleGainedBuffs(string buffType, int stacks)
    {
        AddStacks(buffType, stacks);
    }

    // adds the queens attacks according to the following rules:
    //  1. If the queen has 2 or more stacks of resonate, it will use hatchery.
    //  2. Otherwise, the queen will use another attack. As of now, the only other
    //      attack is fragment, so it will be that one.
    //  3. The queen repeats this process twice, attacking twice in one turn.
    //  In the inspector, assign Hatchery to index 0 of the Queen's deck, and any other
    //     attacks after that.
    public override void AddAttack(List<PlayerClass> players)
    {
        for (int i = 0; i < 2; i++)
        {
            Debug.Log(GetBuffStacks(Resonate.buffName));
            if (GetBuffStacks(Resonate.buffName) >= 2)
            {
                AddAttackFromPool(players, 0);
                ReduceStacks(Resonate.buffName, 2);
            }
            else
            {
                AddAttackFromPool(players, Random.Range(1, pool.Count));
            }
        }
    }

    private void AddAttackFromPool(List<PlayerClass> players, int idx)
    {
        pool[idx].GetComponent<ActionClass>().Target = players[Random.Range(0, players.Count - 1)];
        BattleQueue.BattleQueueInstance.AddEnemyAction(pool[idx].GetComponent<ActionClass>(), this);
        combatInfo.SetCombatSprite(pool[idx].GetComponent<ActionClass>());
        combatInfo.GetComponentInChildren<CombatCardUI>().actionClass = pool[idx].GetComponent<ActionClass>();
    }

}
