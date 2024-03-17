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
    // @Author Anrui; Note by Muhammad: it should be retained that Hurl is inside the availableActions and therefore any modifications should take this into account.
    private GameObject hurlCard;

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

    //@Author Anrui; Instantiate pool without Hurl
    public override void InstantiatePool()
    { 
        for (int i = 0; i < availableActions.Count; i++)
        {
            GameObject toAdd = Instantiate(availableActions[i]);
            ActionClass addedClass = toAdd.GetComponent<ActionClass>();
            addedClass.Origin = this;

            if (addedClass.GetName() == Hurl.HURL_NAME)
            {
                hurlCard = toAdd;   
            } else
            {
                deck.Add(toAdd);
            }
        }
        if (hurlCard == null) 
        {
            Debug.LogWarning("Hurl Card Not Instantiated");
        }
    }
    // @Author Muhammad; excerpt from Andrew
    // if UseHurl then add Hurl this time around otherwise add actions normally.
    // this doesn't have to deal with the "new" conundrum because... follow logic; emphasis on played next turn and don't discard
    public override void AddAttack(List<PlayerClass> players)
    {
        if (UseHurl && hurlCard != null)
        {
            UseHurl = false;
            if (players.Count == 0) return;
            
            ActionClass temporaryHurl = hurlCard.GetComponent<ActionClass>();
            temporaryHurl.Target = players[Random.Range(0, players.Count - 1)];
            BattleQueue.BattleQueueInstance.AddEnemyAction(temporaryHurl, this);
            combatInfo.SetCombatSprite(temporaryHurl);
            combatInfo.GetComponentInChildren<CombatCardUI>().actionClass = temporaryHurl;
        } else
        {
            base.AddAttack(players);
        }
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
