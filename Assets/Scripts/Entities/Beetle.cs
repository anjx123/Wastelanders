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

    public delegate void OnDeathHandler(Beetle victim); // so that queen knows how many beetles are alive
    public static event OnDeathHandler OnDeath;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        myName = "Beetle";
    }

    // Overrides the normal behaviour of adding buffs. Instead, broadcasts for the queen to handle
    public override void AddStacks(string buffType, int stacks)
    {
        if (buffType == Resonate.buffName)
        {
            OnGainBuffs?.Invoke(buffType, stacks);
        } else
        {
            base.AddStacks(buffType, stacks);
        }
    }

    // Adds attack(s) to the bq. Beetles are capable of directing their attacks to crystals
    //  on top of players. Individual beetle subclasses may further override this method to
    //  favour targeting on crystals/players.
    public override void AddAttack(List<PlayerClass> players) {
        if (players.Count == 0) return; // if there are no players, no need to attack crystals
        // add crystals to list of potential targets
        List<EntityClass> targets = players.Cast<EntityClass>().ToList();
        List<EntityClass> crystals = CombatManager.Instance.GetEnemies().OfType<Crystals>().ToList().Cast<EntityClass>().ToList();
        targets.AddRange(crystals);

        // rest is the same
        pool[0].GetComponent<ActionClass>().Target = targets[Random.Range(0, targets.Count)];
        BattleQueue.BattleQueueInstance.AddEnemyAction(pool[0].GetComponent<ActionClass>(), this);
        combatInfo.AddCombatSprite(pool[0].GetComponent<ActionClass>());
        pool.RemoveAt(0);
        if (pool.Count < 1)
        {
            Reshuffle();
        }
    }

    // notifies of death
    public override IEnumerator Die()
    {
        OnDeath?.Invoke(this);
        return base.Die();
    }
}
