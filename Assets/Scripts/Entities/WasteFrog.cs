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
    public override void InstantiateDeck()
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

    public override void DestroyDeck()
    {
        base.DestroyDeck();
        Destroy(hurlCard);
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
            temporaryHurl.Origin = this;
            temporaryHurl.Target = players[Random.Range(0, players.Count - 1)];
            BattleQueue.BattleQueueInstance.AddAction(temporaryHurl);
            combatInfo.AddCombatSprite(temporaryHurl);
        } else
        {
            base.AddAttack(players);
        }
    }
    public override void TakeDamage(EntityClass source, int damage)
    {
        MusicManager.Instance.PlaySFX(MusicManager.SFXList.wastefrog_damage_taken);
        base.TakeDamage(source, damage);
    }
}
