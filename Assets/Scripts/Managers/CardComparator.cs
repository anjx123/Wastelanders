using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.CinemachineTargetGroup;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.Image;

public class CardComparator : MonoBehaviour
{
    public static CardComparator Instance { get; private set; }
    public static readonly float COMBAT_BUFFER_TIME = 1f;
    public delegate IEnumerator DeadEntities();
    public static event DeadEntities PlayEntityDeaths;



    // Awake is called when the script instance is being loaded
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null && Instance != this)
        {
            Destroy(this);
        }

    }

    /*
     * Clashes two cards together handling logic calls and activating the Combat Info
     */
    public IEnumerator ClashCards(ActionClass card1, ActionClass card2)
    {
        int cardOneStaggered;
        CombatManager.Instance.SetCameraCenter(card1.Origin);
        ActivateInfo(card1, card2);
        card1.ApplyEffect();
        card2.ApplyEffect();
        yield return StartCoroutine(ClashBothEntities(card1.Origin, card1.Target)); // for animation purposes 

        card1.RollDice();
        card2.RollDice();
        DeactivateInfo(card1, card2);

        if (IsAttack(card1) && IsAttack(card2))
        {
            cardOneStaggered = ClashCompare(card1, card2);
            //Debug.Log(cardOneStaggered);
            if (cardOneStaggered == 0) //Clash ties
            {
                card1.OnHit(); // For testing purposes, not supposed to be here
            } else if (cardOneStaggered > 0) //Card2 wins clash
            {
                card2.OnHit();
            } else if (cardOneStaggered < 0) //Card1 wins clash
            {
                card1.OnHit();
            }
        } else if (card1.CardType == CardType.Defense)
        {
            card1.Origin.BlockAnimation(); //Blocked stuff here not implemented properly
            
        } else if (card2.CardType == CardType.Defense)
        {
            
        } else
        {
            Debug.LogWarning("You forgot to specify Cardtype");
        }

        
        yield return new WaitForSeconds(COMBAT_BUFFER_TIME);
        if (PlayEntityDeaths != null)
        {
            yield return PlayEntityDeaths();
            PlayEntityDeaths = null;
        }
        DeEmphasizeClashers(card1.Origin, card1.Target);
    }

    //Produces a positive value if Card1 is staggered by Card2
    private int ClashCompare(ActionClass card1, ActionClass card2)
    {
        int cardOneStaggered;
        

        if (card1.getRolledDamage() > card2.getRolledDamage())
        {;
            cardOneStaggered = -1; //Card 1 staggers card 2
        } else if (card1.getRolledDamage() < card2.getRolledDamage())
        {
            cardOneStaggered = 1;
        } else 
        {
            cardOneStaggered = 0;
        }
        return cardOneStaggered;
    }

    /*
 EntityClass origin: Origin of the action card played
 EntityClass target: Target of the action card played
    bufferedRadius: The buffer circle in which the entity will stop before that circle. 

 Purpose: The two clashing enemies come together to clash, their positions will ideally be based off their speed
 Then, whoever wins the clash should stagger the opponent backwards. 
  */
    public static readonly float X_BUFFER = 0.8f;
    private IEnumerator ClashBothEntities(EntityClass origin, EntityClass target)
    {
        EmphasizeClashers(origin, target);
        //The Distance weighting will be calculated based on speeds of the two clashing cards
        Vector3 centeredDistance = (origin.myTransform.position * 0.3f + 0.7f * target.myTransform.position);
        float bufferedRadius = 0.25f;
        float duration = 0.6f;
        
        
        StartCoroutine(origin?.MoveToPosition(HorizontalProjector(centeredDistance, origin.myTransform.position, X_BUFFER), bufferedRadius, duration, centeredDistance));
        yield return StartCoroutine(target?.MoveToPosition(HorizontalProjector(centeredDistance, target.myTransform.position, X_BUFFER), bufferedRadius, duration, centeredDistance));
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
    }
    /*
     * 
     * Projects A Character's position onto the same x-axis as centeredDistnace but is 'xBuffer' x-distance away from the 'centeredDistance'
     */
    private Vector3 HorizontalProjector(Vector3 centeredDistance, Vector3 currentPosition, float xBuffer)
    {
        Vector3 vectorToCenter = (centeredDistance - currentPosition);
        
        return vectorToCenter.x > 0 ?
            currentPosition + vectorToCenter - new Vector3(xBuffer, 0f, 0f) :
            currentPosition + vectorToCenter + new Vector3(xBuffer, 0f, 0f);
    }

    private void ActivateInfo(ActionClass card1, ActionClass card2)
    {

        card1.Origin.ActivateCombatInfo(card1);
        card2.Origin.ActivateCombatInfo(card2);
    }

    private void DeactivateInfo(ActionClass card1, ActionClass card2)
    {
        card1.Origin.DeactivateCombatInfo();
        card2.Origin.DeactivateCombatInfo();
    }

    private void EmphasizeClashers(EntityClass origin, EntityClass target)
    {
        origin.Emphasize();
        target.Emphasize();
    }

    private void DeEmphasizeClashers(EntityClass origin, EntityClass target)
    {
        origin.DeEmphasize();
        target.DeEmphasize();
    }

    private bool IsAttack(ActionClass card)
    {
        return card.CardType == CardType.MeleeAttack || card.CardType == CardType.RangedAttack;
    }
}
