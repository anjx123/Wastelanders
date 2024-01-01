using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityClass : SelectClass
{
    protected int MAX_HEALTH;
    protected int health;
    public HealthBar healthBar;
    public Animator animator;
    public Transform myTransform;
    public int Health
    {
        get { return health; }
        set { health = value; }
    }

    protected Dictionary<string, StatusEffect> statusEffects;

    // Initializes the Dictionary of Buff
    protected void DictInit()
    {
        statusEffects[Focus.buffName] = BuffFactory.GetStatusEffect(Focus.buffName);
        statusEffects[Accuracy.buffName] = BuffFactory.GetStatusEffect(Accuracy.buffName);
    }

    protected List<ActionClass> actionsAvailable;

    protected int id;
    public int Id 
    { 
        get { return id; }
        set { id = value; }
    }

    public virtual void Start()
    {
        healthBar.setMaxHealth(MAX_HEALTH);
        healthBar.setHealth(MAX_HEALTH);

        statusEffects = new Dictionary<string, StatusEffect>();
        DictInit();
    }

    public virtual void TakeDamage(int damage)
    {
        health = Mathf.Clamp(health - damage, 0, MAX_HEALTH);
        healthBar.setHealth(health);
        if (health <= 0)
        {
           // Die();
        }
    }

    /*
     * Usage:
    Vector3 destination: Destination Of the Moving individual
    float radius: Radius is the radius right before the destination the entity will stop at.
    (Can be useful to prevent two enemies from clipping together)
    float duration: Duration of the movement

    Modifies: this.myTransform

    Purpose: Moves this entity to a given location
     */
    public IEnumerator MoveToPosition(Vector3 destination, float radius, float duration)
    {
        Vector3 originalPosition = myTransform.position;
        float elapsedTime = 0f;

        Vector3 diffInLocation = destination - originalPosition;

        float distance = Mathf.Sqrt(diffInLocation.x * diffInLocation.x + diffInLocation.y * diffInLocation.y);
        float maxProportionTravelled = (distance - radius) / distance;

        while (elapsedTime < duration)
        {
            myTransform.position = Vector3.Lerp(originalPosition, destination, elapsedTime / duration * maxProportionTravelled);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }


    /* Requires: "IsStaggered" bool exists on the animator controller attatched to this
     * 
     * Purpose: Plays when the enemy is staggered
     * Pushes the enemy back to staggeredPosition
     * Plays the stagger animation
     * 
     * Params:
     * staggeredPosition is the location we want the enemy to stop at. 
     * 
     * Modifies: this.myTransform.position
     */
    public IEnumerator StaggerBack(Vector3 staggeredPosition)
    {
        Vector3 originalPosition = myTransform.position;
        float elapsedTime = 0f;

        animator.SetBool("IsStaggered", true);
        float duration = animator.GetCurrentAnimatorStateInfo(0).length;

        while (elapsedTime < duration)
        {
            myTransform.position = Vector3.Lerp(originalPosition, staggeredPosition, AnimationCurve(elapsedTime, duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        animator.SetBool("IsStaggered", false);
    }
    private float AnimationCurve(float elapsedTime, float duration)
    {
        float speed = 0.8f; //Lower value is faster
        float power = 5f; //Modifies the curvature of the curve
        return (Mathf.Pow(speed, power) / Mathf.Pow(((-elapsedTime) / duration - speed), power) + 1);
    }

    public virtual void Heal(int val)
    {
        health = Mathf.Clamp(health + val, 0, MAX_HEALTH);
        healthBar.setHealth(health);
    }

    public override void OnMouseDown()
    {
        HighlightManager.OnEntityClicked(this);
    }

    public void Die()
    {
        Debug.Log("Entity: " + id + " has died");
    }
    /*
    // Constructor
    public EntityClass(int health)
    {
        this.health = health;
        this.MAX_HEALTH = health;
    } */

    // Adds the Stacks of the Card to the Relevant Buff Stacks of the Player
    public void AddStacks(ref ActionClass.CardDup dup, string buffType)
    {
        statusEffects[buffType].GainStacks(ref dup);
    }

    // Applies the Stacks of the Specified Buff to the Card Roll Limits
    public void ApplyBuffsToCard(ref ActionClass.CardDup dup, string buffType)
    {

        Debug.Log("add buffs no error initial success" + dup.rollCeiling);
        statusEffects[buffType].ApplyStacks(ref dup);
    }
}
