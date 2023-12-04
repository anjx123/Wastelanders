using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardComparator : MonoBehaviour
{
    public int CompareCards(ActionClass card1, ActionClass card2)
    {
        int cardOneStaggered = 0;
        if (IsAttack(card1) && IsAttack(card2))
        {
            cardOneStaggered = ClashCompare(card1, card2);
            if (cardOneStaggered == 0) //Clash ties
            {
                card1.Origin.AttackAnimation();
                card2.Origin.AttackAnimation();
            } else if (cardOneStaggered > 0) //Card2 wins clash
            {
                //card2.Target.StaggerBack();
                card2.Origin.AttackAnimation();
                card2.Target.TakeDamage(card2.Damage);
                
            } else if (cardOneStaggered < 0) //Card1 wins clash
            {
                card1.Origin.AttackAnimation();
                //card1.Target.StaggerBack();
                card1.Target.TakeDamage(card1.Damage);

            }
        } else if (card1.CardType == CardType.Defense)
        {
            card2.Damage = card2.Damage - card1.Block;
            cardOneStaggered = 1;

            card1.Origin.BlockAnimation();
            //card2.Target.StaggerBack();
            card2.Origin.AttackAnimation();
            card2.Target.TakeDamage(card2.Damage);
        } else if (card2.CardType == CardType.Defense)
        {
            card1.Damage = card1.Damage - card2.Block;
            cardOneStaggered = 1;

            card1.Origin.AttackAnimation();
            //card1.Target.StaggerBack();
            card1.Target.TakeDamage(card1.Damage);
        }

        return cardOneStaggered;
    }

    private int ClashCompare(ActionClass card1, ActionClass card2)
    {
        int cardOneStaggered = 0;

        if (card1.getDamage() > card2.getDamage())
        {
            card2.setDamage(0);
            cardOneStaggered = -1;
        } else if (card1.getDamage() < card2.getDamage())
        {
            card1.setDamage(0);
            cardOneStaggered = 1;
        } else 
        {
            card1.setDamage(0);
            card2.setDamage(0);
            cardOneStaggered = 0;
        }
        return cardOneStaggered;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private bool IsAttack(ActionClass card)
    {
        return card.CardType == CardType.MeleeAttack || card.CardType == CardType.RangedAttack;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
