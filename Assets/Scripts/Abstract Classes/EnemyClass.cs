using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        CombatManager.Instance.AddEnemy(this);
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

    public virtual void AddAttack(List<PlayerClass> players)
    {
        if (players.Count == 0) return;
        if (pool.Count == 0) return;
        pool[0].GetComponent<ActionClass>().Target = players[Random.Range(0, players.Count)]; // excludes the last value 
        pool[0].GetComponent<ActionClass>().Origin = this;
        BattleQueue.BattleQueueInstance.AddAction(pool[0].GetComponent<ActionClass>());
        combatInfo.AddCombatSprite(pool[0].GetComponent<ActionClass>());
        pool.RemoveAt(0);
        if (pool.Count < 1)
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
            int idx = Random.Range(0, temp.Count);
            pool.Add(temp[idx]);
            temp.RemoveAt(idx);
        }
    }

    public override IEnumerator Die()
    {
        int runDistance = 10;
        BattleQueue.BattleQueueInstance.RemoveAllInstancesOfEntity(this);
        CombatManager.Instance.RemoveEnemy(this);
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
        FaceLeft();
    }

    public override void PerformSelection(List<EntityClass> playerTeam, List<EntityClass> neutralTeam, List<EntityClass> enemyTeam)
    {

    }
}
