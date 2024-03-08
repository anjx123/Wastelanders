using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingDummy : EnemyClass
{

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        MaxHealth = 10;
        Health = MaxHealth;
        myName = "Le Dummy";
    }

    public override IEnumerator Die()
    {
        BattleQueue.BattleQueueInstance.RemoveAllInstancesOfEntity(this);
        CombatManager.Instance.RemoveEnemy(this);
        isDead = true;
        yield break;
    }
}
