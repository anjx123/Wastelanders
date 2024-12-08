using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIves : EnemyClass
{
    public override void Start()
    {
        base.Start();
        MaxHealth = 25;
        Health = MaxHealth;
        myName = "Le Ives";
    }

    public void InjectDeck(List<GameObject> actions)
    {
        foreach (GameObject action in actions)
        {
            GameObject toAdd = Instantiate(action);
            ActionClass addedClass = toAdd.GetComponent<ActionClass>();
            toAdd.transform.position = new Vector3(-10, -10, -10);
            addedClass.Origin = this;

            deck.Add(toAdd);
            pool.Add(toAdd);
        }
    }

    public override IEnumerator Die()
    {
        BattleQueue.BattleQueueInstance.RemoveAllInstancesOfEntity(this);
        if (HasAnimationParameter("IsStaggered"))
        {
            animator.SetBool("IsStaggered", true);
        }
        yield break;
    }

}
