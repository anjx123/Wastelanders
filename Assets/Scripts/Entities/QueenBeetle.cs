using Cards.EnemyCards.FrogCards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenBeetle : EnemyClass
{
    private int MAX_BEETLES;
    [SerializeField]
    private GameObject[] beetlePrefabs = new GameObject[3];
    private Beetle[] availability;

    // spawn locations for the beetles
    private Vector3[] beetleLocations = new Vector3[4];

    private const float BEETLE_SCALING = 0.6f;

    public void Awake()
    {
        MAX_BEETLES = beetleLocations.Length;
        availability = new Beetle[MAX_BEETLES];
        for (int i = 0; i < availability.Length; i++)
        {
            availability[i] = null;
        }
    }

    public void InheritChild(List<Beetle> guardBeetles)
    {
        for (int i = 0; i < guardBeetles.Count; ++i)
        {
            availability[i] = guardBeetles[i];
            beetleLocations[i] = guardBeetles[i].transform.position;
        }
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        MaxHealth = 50;
        Health = MaxHealth;
        myName = "The Queen";
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        Beetle.OnGainBuffs += HandleGainedBuffs;
        OnEntityDeath += HandleBeetleDied;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        Beetle.OnGainBuffs -= HandleGainedBuffs;
        OnEntityDeath -= HandleBeetleDied;
    }

    // event handler for Beetle.OnGainBuffs. This is called whenever a beetle tries to
    // add a buff.
    // Adds the stacks that were directed to the beetle to the Queen instead.
    private void HandleGainedBuffs(string buffType, int stacks, Beetle beetle)
    {
        if (buffType == Resonate.buffName)
        {
            AddStacks(buffType, stacks);
            beetle.ReduceStacks(buffType, stacks);
        }
    }

    private void HandleBeetleDied(EntityClass victim)
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
    public override void AddAttack(List<EntityClass> targets)
    {
        bool usedSpawnThisRound = false;
        var hatchery = deck[0];
        for (int i = 0; i < 2; i++)
        {
            if (GetBuffStacks(Resonate.buffName) >= 2 && FindFirstOpenSlot() != -1 && (!usedSpawnThisRound || NumberOfAvailableSlots() > 1)) //Last condition fixes a bug where the queen can try to spawn 2 beetles but then hit the max spawn cap
            {
                AttackWith(hatchery, AttackTargetCalculator(targets));
                ReduceStacks(Resonate.buffName, 2);
                UpdateBuffs();
                usedSpawnThisRound = true;
            }
            else
            {
                AttackWith(deck[Random.Range(1, deck.Count)], AttackTargetCalculator(targets));
            }
        }
    }

    int NumberOfAvailableSlots()
    {
        int number = 0;
        for (int i = 0; i < availability.Length; i++)
        {
            if (availability[i] == null)
            {
                ++number;
            }
        }
        Debug.Log(number);
        return number;
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
        GameObject beetle = Instantiate(beetlePrefabs[Random.Range(0, beetlePrefabs.Length)]);
        beetle.transform.SetParent(transform.parent);
        beetle.transform.localScale *= BEETLE_SCALING;
        beetle.transform.position = beetleLocations[slot];
        availability[slot] = beetle.GetComponent<Beetle>();
    }

    private float cycleScaling = 2f; // Higher the number, the faster one phase is 
    private float bobbingAmount = 0.1f; //Amplitude
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
