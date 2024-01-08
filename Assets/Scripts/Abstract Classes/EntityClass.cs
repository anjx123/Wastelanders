using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.UI.Image;

public abstract class EntityClass : SelectClass
{
    protected int MAX_HEALTH;
    protected int health;
    public HealthBar healthBar;
    public Animator animator;
    public Transform myTransform;
    public CombatInfo combatInfo;

    private static readonly float Z_LAYER = 0;

    protected Vector3 initalPosition;
    public int Health
    {
        get { return health; }
        set { health = value; }
    }

    protected Dictionary<string, StatusEffect> statusEffects;



    protected List<ActionClass> actionsAvailable;

    protected int id;
    public int Id 
    { 
        get { return id; }
        set { id = value; }
    }


    private bool grewLarger; //Checks if entity was highlighted first to ensure proper dehighlighting. 

    public virtual void Start()
    {

        healthBar.setMaxHealth(MAX_HEALTH);
        healthBar.setHealth(MAX_HEALTH);
        initalPosition = myTransform.position;
        statusEffects = new Dictionary<string, StatusEffect>();

        DeEmphasize();
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

        if ((Vector2) diffInLocation == Vector2.zero) yield break;

        float distance = Mathf.Sqrt(diffInLocation.x * diffInLocation.x + diffInLocation.y * diffInLocation.y);
        float maxProportionTravelled = (distance - radius) / distance;

        UpdateFacing(diffInLocation, CardComparator.X_BUFFER);

        if (HasParameter("IsMoving", animator))
        {
            animator.SetBool("IsMoving", true);
        }

        while (elapsedTime < duration)
        {
            myTransform.position = Vector3.Lerp(originalPosition, destination, elapsedTime / duration * maxProportionTravelled);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (HasParameter("IsMoving", animator))
        {
            animator.SetBool("IsMoving", false);
        }
    }

    public abstract void FaceRight();

    public abstract void FaceLeft();

    public bool IsFacingRight()
    {
        return combatInfo.IsFacingRight();
    }

    /*
     * Purpose: Updates the entitiy's direction to face a target. (Target.position - my position)
     * diffInLocation: Will face the entity Right if the Target is to its right (positive diffInLocation)
        Left if other way around
        ComparingBuffer: Adds a buffer where if the  (abs) |x-distance| travelled is smaller than comparingBuffer, No flip is made   
        Note: If you want to reverse the results, add a negative to diffInLocation before calling.
     */
    public void UpdateFacing(Vector3 diffInLocation, float comparingBuffer)
    {
        if (diffInLocation.x > comparingBuffer)
        {
            FaceRight();
        }
        else if (diffInLocation.x < -(comparingBuffer))
        {
            FaceLeft();
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

        Vector3 diffInLocation = staggeredPosition - originalPosition;
        if ((Vector2)diffInLocation == Vector2.zero) yield break;
        UpdateFacing(-diffInLocation, 0);



        if (HasParameter("IsStaggered", animator))
        {
            animator.SetBool("IsStaggered", true);
        }

        float duration = animator.GetCurrentAnimatorStateInfo(0).length;

        while (elapsedTime < duration)
        {
            myTransform.position = Vector3.Lerp(originalPosition, staggeredPosition, AnimationCurve(elapsedTime, duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (HasParameter("IsStaggered", animator))
        {
            animator.SetBool("IsStaggered", false);
        }
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

    public abstract IEnumerator ResetPosition();

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
    public void AddStacks(string buffType, int stacks)
    {
        if (!statusEffects.ContainsKey(buffType)) 
        { 
            statusEffects[buffType] = BuffFactory.GetStatusEffect(buffType);
        }
        statusEffects[buffType].GainStacks(stacks);
        UpdateBuffs();
    }

    // Applies the Stacks of the Specified Buff to the Card Roll Limits
    public void ApplyBuffsToCard(ref ActionClass.CardDup dup, string buffType)
    {
        if (!statusEffects.ContainsKey(buffType))
        {
            statusEffects[buffType] = BuffFactory.GetStatusEffect(buffType);
        }
        statusEffects[buffType].ApplyStacks(ref dup);
    }

    // Applies the Stacks of all Buffs to the Card Roll Limits
    public void ApplyAllBuffsToCard(ref ActionClass.CardDup dup)
    {
        foreach (string buff in  statusEffects.Keys)
        {
            ApplyBuffsToCard(ref dup, buff);
        }
    }

    public int GetBuffStacks(string s)
    {
        if (statusEffects.ContainsKey(s))
        {
            return statusEffects[s].Stacks;
        }
        return 0;
    }

    public virtual void AttackAnimation(string animationName)
    {
        if (HasParameter(animationName, animator))
        {
            animator.SetTrigger(animationName);
        }
    }

    public virtual void BlockAnimation()
    {
        //Todo
        //Implement Block Animation
    }

    public static bool HasParameter(string paramName, Animator animator)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName) return true;
        }
        return false;
    }

    public void ActivateCombatInfo(ActionClass actionClass)
    {
        combatInfo.SetCombatSprite(actionClass);
    }

    public void DeactivateCombatInfo()
    {
        combatInfo.DeactivateCombatSprite();
    }
    //Increases this Entity Class' sorting layer (negative number is higher up)
    public void Emphasize()
    {
        Vector3 largeTransform = transform.position;
        largeTransform.z = Z_LAYER - 1;
        transform.position = largeTransform;
    }

    //Decreases this Entity Class' sorting layer. (Standardizes Sorting Layers for entities)
    public void DeEmphasize()
    {
        Vector3 largeTransform = transform.position;
        largeTransform.z = Z_LAYER;
        transform.position = largeTransform;
    }

    public void SetDice(int value)
    {
        combatInfo.SetDice(value);
    }

    public void UpdateBuffs()
    {
        combatInfo.UpdateBuffs(statusEffects);
    }

    public override void OnMouseEnter()
    {
        if (CombatManager.Instance.CanHighlight())
        {
            myTransform.localScale += new Vector3((float)0.05, (float)0.05, 0);
            grewLarger = true;
        }
    }

    public override void OnMouseExit()
    {
        if (grewLarger)
        {
            myTransform.localScale -= new Vector3((float)0.05, (float)0.05, 0);
            grewLarger = false;
        }
    }
}
