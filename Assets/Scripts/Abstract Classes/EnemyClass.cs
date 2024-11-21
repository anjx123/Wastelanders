using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyClass : EntityClass
{
    // This is the deck of the enemy, which does not change as they reshuffle/play cards.
    protected List<GameObject> deck = new List<GameObject>();

    // This is the pool, which is initialized to the deck in a random order. Then cards will be taken from here. Cards are taken from index 0.
    protected List<GameObject> pool = new List<GameObject>();
    
    // Initialized in editor
    public List<GameObject> availableActions;

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

    public virtual void AddAttack(List<EntityClass> players)
    {
        if (players.Count == 0 || pool.Count == 0) return;

        var action = pool[0].GetComponent<ActionClass>();
        action.Target = players[UnityEngine.Random.Range(0, players.Count)];
        action.Origin = this;

        BattleQueue.BattleQueueInstance.AddAction(action);
        combatInfo.AddCombatSprite(action);

        pool.RemoveAt(0);

        if (pool.Count == 0)
        {
            Reshuffle();
        }
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
