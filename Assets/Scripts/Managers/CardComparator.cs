using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.CinemachineTargetGroup;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.Image;

public class CardComparator : MonoBehaviour
{
    public static CardComparator Instance { get; private set; }

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

        DontDestroyOnLoad(gameObject);
    }
    public IEnumerator ClashCards(ActionClass card1, ActionClass card2)
    {
        int cardOneStaggered;
        ActivateInfo(card1, card2);
        yield return StartCoroutine(ClashBothEntities(card1.Origin, card1.Target));

        card1.RollDice();
        card2.RollDice();

        if (IsAttack(card1) && IsAttack(card2))
        {
            cardOneStaggered = ClashCompare(card1, card2);
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
            card2.Target.TakeDamage(card2.Damage);
        } else if (card2.CardType == CardType.Defense)
        {
            card1.Target.TakeDamage(card1.Damage);
        }
    }

    //Produces a positive value if Card1 is staggered by Card2
    private int ClashCompare(ActionClass card1, ActionClass card2)
    {
        int cardOneStaggered;

        if (card1.Damage > card2.Damage)
        {;
            cardOneStaggered = -1; //Card 1 staggers card 2
        } else if (card1.Damage < card2.Damage)
        {
            cardOneStaggered = 1;
        } else 
        {
            cardOneStaggered = 0;
        }
        return cardOneStaggered;
    }

    //Gives ownership of the stagger Entities animation to the card comparator
    public void StartStagger(EntityClass origin, EntityClass target, float percentageDone)
    {
        StartCoroutine(StaggerEntities(origin, target, percentageDone));
    }

    /* 
    Purpose: Staggers an entity back 
    origin: The origin of the damage/attack is coming from
    target: The target being staggered back
    percentageDone: Percentage health done to the target
     */
    private IEnumerator StaggerEntities(EntityClass origin, EntityClass target, float percentageDone)
    {
        Vector2 directionVector = target.myTransform.position - origin.myTransform.position;

        Vector2 normalizedDirection = directionVector.normalized;
        float staggerPower = StaggerPowerCalculation(percentageDone);

        yield return StartCoroutine(target.StaggerBack(target.myTransform.position + (Vector3)normalizedDirection * staggerPower));
        SelectionMode();
    }

    //Calculates the power of the stagger based on the percentage health done
    private float StaggerPowerCalculation(float percentageDone)
    {
        float minimumPush = 0.8f;
        float pushSlope = 1.8f;
        float percentageUntilMaxPush = 1f / 3f; //Reaches Max push at 33% hp lost
        return minimumPush + pushSlope * Mathf.Clamp(percentageDone / percentageUntilMaxPush, 0f, 1.5f);
    }

    //Transitions the gamestate to selection mode.
    private void SelectionMode()
    {
        CombatManager.Instance.GameState = GameState.SELECTION;
    }


    /*
 EntityClass origin: Origin of the action card played
 EntityClass target: Target of the action card played
    bufferedRadius: The buffer circle in which the entity will stop before that circle. 

 Purpose: The two clashing enemies come together to clash, their positions will ideally be based off their speed
 Then, whoever wins the clash should stagger the opponent backwards. 
  */
    private IEnumerator ClashBothEntities(EntityClass origin, EntityClass target)
    {
        CombatManager.Instance.GameState = GameState.FIGHTING;
        //The Distance weighting will be calculated based on speeds of the two clashing cards
        Vector2 centeredDistance = (origin.myTransform.position * 0.3f + 0.7f * target.myTransform.position);
        float bufferedRadius = 0.25f;
        float duration = 0.6f;
        float xBuffer = 0.6f;
        StartCoroutine(origin.MoveToPosition(HorizontalProjector(centeredDistance, origin.myTransform.position, xBuffer), bufferedRadius, duration));
        yield return StartCoroutine(target.MoveToPosition(HorizontalProjector(centeredDistance, target.myTransform.position, xBuffer), bufferedRadius, duration));
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
    }
    /*
     * 
     * Projects A Character's position onto the same x-axis as centeredDistnace but is 'xBuffer' x-distance away from the 'centeredDistance'
     */
    private Vector2 HorizontalProjector(Vector2 centeredDistance, Vector2 currentPosition, float xBuffer)
    {
        Vector2 vectorToCenter = (centeredDistance - currentPosition);

        return vectorToCenter.x > 0 ?
            currentPosition + vectorToCenter - new Vector2(xBuffer, 0) :
            currentPosition + vectorToCenter + new Vector2(xBuffer, 0);
    }

    private void ActivateInfo(ActionClass card1, ActionClass card2)
    {
        card1.Origin.ActivateCombatInfo(card1);
        card2.Origin.ActivateCombatInfo(card2);
    }

    private bool IsAttack(ActionClass card)
    {
        return card.CardType == CardType.MeleeAttack || card.CardType == CardType.RangedAttack;
    }
}
