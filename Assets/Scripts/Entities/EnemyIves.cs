using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIves : EnemyClass
{
    public override void Start()
    {
        base.Start();
        CombatManager.Instance.RemoveEnemy(this);
        MaxHealth = 20;
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

            Reshuffle();
        }
    }

    public override IEnumerator Die()
    {
        BattleQueue.BattleQueueInstance.RemoveAllInstancesOfEntity(this);
        CombatManager.Instance.RemoveEnemy(this);
        if (HasParameter("IsStaggered", animator))
        {
            animator.SetBool("IsStaggered", true);
        }
        yield break;
    }

    public void UnTargetable()
    {
        boxCollider.enabled = false;
    }

    public void Targetable()
    {
        boxCollider.enabled = true;
    }
}