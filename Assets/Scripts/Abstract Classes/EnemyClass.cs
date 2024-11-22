using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public abstract class EnemyClass : EntityClass
{
    // This is the deck of the enemy, which does not change as they reshuffle/play cards.
    protected List<GameObject> deck = new List<GameObject>();

    // This is the pool, which is initialized to the deck in a random order. Then cards will be taken from here. Cards are taken from index 0.
    protected List<GameObject> pool = new List<GameObject>();
    
    // Initialized in editor
    public List<GameObject> availableActions;

    public delegate EntityClass AttackTargetDelegate(List<EntityClass> targets);

    public AttackTargetDelegate AttackTargetCalculator { get; set; } = GetAttackTarget;

    public override void Start()
    {
        if (Team == EntityTeam.NoTeam) Team = EntityTeam.EnemyTeam;
        base.Start();
        InstantiateDeck();

        Reshuffle();
    }

    public virtual void InstantiateDeck()
    {
        for (int i = 0; i < availableActions.Count; i++)
        {
            GameObject toAdd = Instantiate(availableActions[i]);
            ActionClass addedClass = toAdd.GetComponent<ActionClass>();
            addedClass.Origin = this;

            deck.Add(toAdd);
        }
    }

    //An attack that pops the top card on the pool and plays it. use this when you attack with pool.
    public virtual void AddAttack(List<EntityClass> targets)
    {
        if (targets.Count == 0 || pool.Count == 0) return;

        AttackWith(pool[0], AttackTargetCalculator(targets));
        pool.RemoveAt(0);

        if (pool.Count == 0)
        {
            Reshuffle();
        }
    }

    // Use this if you would like to directly attack using deck
    public void AttackWith(GameObject attack, EntityClass target)
    {
        var action = attack.GetComponent<ActionClass>();
        action.Target = target;
        action.Origin = this;
        combatInfo.AddCombatSprite(action);
        BattleQueue.BattleQueueInstance.AddAction(action);
    }

    private static EntityClass GetAttackTarget(List<EntityClass> targets)
    {
        return targets[UnityEngine.Random.Range(0, targets.Count)];
    }


    protected virtual void Reshuffle()
    {
        List<GameObject> temp = new List<GameObject>();
        for (int i = 0; i < deck.Count; i++)
        {
            temp.Add(deck[i]);
        }

        while (temp.Count > 0)
        {
            int idx = UnityEngine.Random.Range(0, temp.Count);
            pool.Add(temp[idx]);
            temp.RemoveAt(idx);
        }
    }

    public override IEnumerator Die()
    {
        int runDistance = 10;
        BattleQueue.BattleQueueInstance.RemoveAllInstancesOfEntity(this);
        DestroyDeck();
        yield return StartCoroutine(MoveToPosition(myTransform.position + new Vector3(runDistance, 0, 0), 0, 0.8f));
        this.gameObject.SetActive(false);
    }

    public virtual void DestroyDeck()
    {
        foreach (GameObject card in deck)
        {
            Destroy(card);
        }
        deck.Clear();
        pool.Clear();
    }

    public override IEnumerator ResetPosition()
    {
        yield return StartCoroutine(MoveToPosition(initialPosition, 0f, 0.8f));
        FaceOpponent();
    }

    public override void PerformSelection()
    {
        AddAttack(GetOpponents());
        StartCoroutine(ResetPosition());
    }

    protected virtual List<EntityClass> GetOpponents()
    {
        return Team switch
        {
            EntityTeam.PlayerTeam => new List<EntityClass>(CombatManager.Instance.GetEnemies()),
            EntityTeam.EnemyTeam => new List<EntityClass>(CombatManager.Instance.GetPlayers()),
            EntityTeam.NeutralTeam => new(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
