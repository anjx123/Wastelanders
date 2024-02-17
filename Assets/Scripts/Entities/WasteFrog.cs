using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class WasteFrog : EnemyClass
{
    // @Author Muhammad
    private bool useHurl = false;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        MaxHealth = 15;
        Health = MaxHealth;
        myName = "Le Frog";
    }

    
    // @Author Muhammad
    public bool UseHurl
    {
        get => useHurl;
        set {
            if (value == useHurl)
            {
                throw new System.Exception("Not logically possible. "); 
            }
            useHurl = value;
        }
    }

    // @Author Muhammad; excerpt from Andrew
    // if UseHurl then add Hurl this time around otherwise add actions normally.
    // this doesn't have to deal with the "new" conundrum because... follow logic; emphasis on played next turn and don't discard
    public override void AddAttack(List<PlayerClass> players)
    {
        if (UseHurl)
        {
            UseHurl = false;
            if (players.Count == 0) return;
            ActionClass temporaryHurl;
            foreach(GameObject a in deck)
            {
                if (a.GetComponent<ActionClass>().GetName() == "Hurl")
                {
                    temporaryHurl = a.GetComponent<ActionClass>();
                    temporaryHurl.Target = players[Random.Range(0, players.Count - 1)];
                    BattleQueue.BattleQueueInstance.AddEnemyAction(temporaryHurl, this);
                    combatInfo.SetCombatSprite(a.GetComponent<ActionClass>());
                    combatInfo.GetComponentInChildren<CombatCardUI>().actionClass = a.GetComponent<ActionClass>();

                }
            }
        } else
        {
            base.AddAttack(players);
        }
    }
}
