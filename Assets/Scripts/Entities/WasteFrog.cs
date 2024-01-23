using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class WasteFrog : EnemyClass
{
    // Initialized in editor
    public List<GameObject> availableActions;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        MaxHealth = 15;
        Health = MaxHealth;
        animator = GetComponent<Animator>();
        myTransform = GetComponent<Transform>();
        myName = "Le Frog";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of frog
        for (int i = 0; i < availableActions.Count; i++)
        {
            GameObject toAdd = Instantiate(availableActions[i]);
            toAdd.GetComponent<ActionClass>().Origin = this;
            deck.Add(toAdd);
        }
        
        Reshuffle();

    }

    /*  Contains: 1x Slap, 1x Flail
     * 
     */
    public override void AddAttack(List<PlayerClass> players)
    {
        pool[0].GetComponent<ActionClass>().Target = players[Random.Range(0, players.Count - 1)];
        BattleQueue.BattleQueueInstance.AddEnemyAction(pool[0].GetComponent<ActionClass>(), this);
        combatInfo.SetCombatSprite(pool[0].GetComponent<ActionClass>());
        pool.RemoveAt(0);
        if (pool.Count < 1)
        {
            Reshuffle();
        }
    }

    /*  Reshuffles the deck. Should be called on start (so the enemy can display its first attack), and whenever the enemy runs out of
     *  attacks.
     *  REQUIRES: pool should be empty! But it shouldn't break anything, just mess up the enemy's order of attacks
     *  MODIFIES: pool 
     */
    private void Reshuffle()
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


}
