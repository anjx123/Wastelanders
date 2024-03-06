using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class Beetle : EnemyClass
{
    public delegate void GainedBuffsHandler(string buffType, int stacks); // queen should subscribe to this
    public static event GainedBuffsHandler OnGainBuffs;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        myName = "Beetle";
    }

    // Overrides the normal behaviour of adding buffs. Instead, broadcasts for the queen to handle
    public override void AddStacks(string buffType, int stacks)
    {
        OnGainBuffs?.Invoke(buffType, stacks);
    }

    public override void AddAttack(List<PlayerClass> players) {
        if (players.Count == 0) return; // if there are no players, no need to attack crystals
        // add crystals to list of potential targets
        List<EntityClass> targets = players.Cast<EntityClass>().ToList();
        List<EntityClass> crystals = CombatManager.Instance.getCrystals().Cast<EntityClass>().ToList();
        targets.AddRange(crystals);

        for (int i = 0; i < 100; i++)
        {
            Debug.Log(Random.Range(0, targets.Count));
        }
        // rest is the same
        pool[0].GetComponent<ActionClass>().Target = targets[Random.Range(0, targets.Count)];
        BattleQueue.BattleQueueInstance.AddEnemyAction(pool[0].GetComponent<ActionClass>(), this);
        combatInfo.SetCombatSprite(pool[0].GetComponent<ActionClass>());
        combatInfo.GetComponentInChildren<CombatCardUI>().actionClass = pool[0].GetComponent<ActionClass>();
        pool.RemoveAt(0);
        if (pool.Count < 1)
        {
            Reshuffle();
        }
    }
}
