using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class QueenBeetle : EnemyClass
{
    private int MAX_BEETLES;
    [SerializeField]
    private GameObject[] beetlePrefabs = new GameObject[3];
    private Beetle[] availability;

    [SerializeField]
    public GameObject enemyContainer;

    // spawn locations for the beetles
    [SerializeField] private Transform[] beetleLocations;

    private const float BEETLE_SCALING = 0.6f;

    public void Awake()
    {
        MAX_BEETLES = beetleLocations.Length;
        availability = new Beetle[MAX_BEETLES];
        for (int i = 0; i < availability.Length; i++)
        {
            availability[i] = null;
        }
        for (int i = 0; i < beetlePrefabs.Length; ++i)
        {
            GameObject beetle = Instantiate(beetlePrefabs[i]);
            beetle.transform.SetParent(enemyContainer.transform);
            beetle.transform.localScale *= BEETLE_SCALING;
            beetle.transform.position = beetleLocations[i].position;
            availability[i] = beetle.GetComponent<Beetle>();
        }
    }
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        MaxHealth = 30;
        Health = MaxHealth;
        myName = "The Queen";

        Beetle.OnGainBuffs += HandleGainedBuffs;
        Beetle.OnDeath += HandleBeetleDied;
    }

    public override IEnumerator Die()
    {
        Beetle.OnGainBuffs -= HandleGainedBuffs;
        Beetle.OnDeath -= HandleBeetleDied;
        return base.Die();
    }

    // event handler for Beetle.OnGainBuffs. This is called whenever a beetle tries to
    // add a buff.
    // Adds the stacks that were directed to the beetle to the Queen instead.
    private void HandleGainedBuffs(string buffType, int stacks)
    {
        AddStacks(buffType, stacks);
    }

    private void HandleBeetleDied(Beetle victim)
    {
        for (int i = 0; i < availability.Length; i++)
        {
            if (availability[i] == victim)
            {
                availability[i] = null;
            }
        }
    }

    // returns the index of the first "null" element in availability. Returns -1 if filled.
    private int FindFirstOpenSlot()
    {
        for (int i = 0; i < availability.Length; i++)
        {
            if (availability[i] == null)
            {
                return i;
            }
        }
        return -1;
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
            if (GetBuffStacks(Resonate.buffName) >= 2 && FindFirstOpenSlot() != -1)
            {
                AddAttackFromPool(players, 0); // hatchery
                ReduceStacks(Resonate.buffName, 2);
                UpdateBuffs();
            }
            else
            {
                AddAttackFromPool(players, Random.Range(1, pool.Count));
            }
        }
    }

    // helper function for AddAttack
    private void AddAttackFromPool(List<PlayerClass> players, int idx)
    {
        pool[idx].GetComponent<ActionClass>().Target = players[Random.Range(0, players.Count)];
        BattleQueue.BattleQueueInstance.AddEnemyAction(pool[idx].GetComponent<ActionClass>(), this);
        combatInfo.SetCombatSprite(pool[idx].GetComponent<ActionClass>());
        combatInfo.GetComponentInChildren<CombatCardUI>().actionClass = pool[idx].GetComponent<ActionClass>();
    }

    // Summons a beetle at random. Called by Hatchery on-hit.
    public void SummonBeetle()
    {
        int slot = FindFirstOpenSlot();
        if (slot == -1)
        {
            Debug.Log("BEETLE OVERLOAD!!! (too many beetles)");
            return;
        }
        if (enemyContainer == null)
        {
            Debug.Log("you forgot to assign enemyContainer in queen");
            return;
        }
        GameObject beetle = Instantiate(beetlePrefabs[Random.Range(0, beetlePrefabs.Length)]);
        beetle.transform.SetParent(enemyContainer.transform);
        beetle.transform.localScale *= BEETLE_SCALING;
        beetle.transform.position = beetleLocations[slot].position;
        availability[slot] = beetle.GetComponent<Beetle>();
    }

    protected override void Reshuffle()
    {
        List<GameObject> temp = new List<GameObject>();
        for (int i = 0; i < deck.Count; i++)
        {
            temp.Add(deck[i]);
        }

        pool = temp;
        return;
    }
    public float cycleScaling = 2f; // Higher the number, the faster one phase is 
    public float bobbingAmount = 0.1f; //Amplitude
    private float timer = 0;
    private float verticalOffset = 0;

    void Update()
    {
        float previousOffset = verticalOffset;
        float waveslice = Mathf.Sin(cycleScaling * timer);
        timer += Time.deltaTime;
        if (timer > Mathf.PI * 2)
        {
            timer = timer - (Mathf.PI * 2);
        }

        verticalOffset = waveslice * bobbingAmount;
        float translateChange = verticalOffset - previousOffset;
        transform.position = new Vector3(transform.position.x, transform.position.y + translateChange, transform.position.z);
    }

}
