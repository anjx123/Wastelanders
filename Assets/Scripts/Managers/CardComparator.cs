using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public int CompareCards(ActionClass card1, ActionClass card2)
    {
        int cardOneStaggered = 0;
        int duration = 2;
        ClashBothEntities(card1.Origin, card1.Target);

        if (IsAttack(card1) && IsAttack(card2))
        {
            cardOneStaggered = ClashCompare(card1, card2);
            if (cardOneStaggered == 0) //Clash ties
            {
                card1.Origin.AttackAnimation();
                card2.Origin.AttackAnimation();
                StartCoroutine(GeneralTimer(duration + 1f, StaggerEntities, new KeyValuePair<EntityClass, EntityClass>(card2.Origin, card2.Target)));
            } else if (cardOneStaggered > 0) //Card2 wins clash
            {

                
                card2.Origin.AttackAnimation();
                card2.OnHit();
                
            } else if (cardOneStaggered < 0) //Card1 wins clash
            {
                card1.Origin.AttackAnimation();
                //card1.Target.StaggerBack();
                card1.OnHit();

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
            card1.Damage -= card2.Block;
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

        if (card1.Damage > card2.Damage)
        {
            card2.Damage = 0;
            cardOneStaggered = -1;
        } else if (card1.Damage < card2.Damage)
        {
            card1.Damage = 0;
            cardOneStaggered = 1;
        } else 
        {
            card1.Damage = 0;
            card2.Damage = 0;
            cardOneStaggered = 0;
        }
        return cardOneStaggered;
    }


    /*
     * A Generic Timer function that performs a task when it is completed. 
     * A delegate is defined here and is essentially a function pointer to a call back. Yippee !!
     * 
     * float duration: Duration until function is called
     * TimerCompletedTask DoFinishedTask: A function pointer to a callback
     * object parameter: generic paramter to pass in to the delegate
     */
    private delegate void TimerCompletedTask(object parameter);
    private IEnumerator GeneralTimer(float duration, TimerCompletedTask DoFinishedTask, object parameter)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Debug.Log("Timer is up, finished task is executing");
        DoFinishedTask(parameter);
    }

    /*
     Takes in a pair of entities with Key being the origin and value being the Target
    Then it calculates a direction vector and staggers the Target bacl from the Origin.
     */
    private void StaggerEntities(object pair)
    {
        KeyValuePair<EntityClass, EntityClass> pairOfEntities = (KeyValuePair<EntityClass, EntityClass>)pair;
        Vector2 directionVector = pairOfEntities.Value.myTransform.position - pairOfEntities.Key.myTransform.position;

        Vector2 normalizedDirection = directionVector.normalized;
        float staggerPower = 2f; //Depending on percentage health lost

        StartCoroutine(pairOfEntities.Value.StaggerBack(pairOfEntities.Value.myTransform.position + (Vector3)normalizedDirection * staggerPower));
    }

    /*
 EntityClass origin: Origin of the action card played
 EntityClass target: Target of the action card played

 Purpose: The two clashing enemies come together to clash, their positions will ideally be based off their speed
 Then, whoever wins the clash should stagger the opponent backwards. 
  */
    private void ClashBothEntities(EntityClass origin, EntityClass target)
    {
        //The Distance weighting will be calculated based on speeds of the two clashing cards
        Vector2 centeredDistance = (origin.myTransform.position * 0.3f + 0.7f * target.myTransform.position);
        float bufferedRadius = 0.6f;
        float duration = 0.6f;
        StartCoroutine(origin.MoveToPosition(centeredDistance, bufferedRadius, duration));
        StartCoroutine(target.MoveToPosition(centeredDistance, bufferedRadius, duration));
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
