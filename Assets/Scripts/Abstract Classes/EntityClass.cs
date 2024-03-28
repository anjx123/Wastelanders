using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardComparator;
using static StatusEffect;

public abstract class EntityClass : SelectClass
{
    private float PLAY_RUNNING_ANIMATION_DELTA = 0.03f; //Represents how little change in position we should at least see before playing running animation
    protected int MAX_HEALTH;
    [SerializeField]
    protected int MaxHealth
    {
        get { return MAX_HEALTH; }
        set
        {
            MAX_HEALTH = value;
            combatInfo.SetMaxHealth(MAX_HEALTH);
        }
    }


    private int health;
    public Animator animator;
    public CombatInfo combatInfo;

    [SerializeField] protected BoxCollider boxCollider;
    protected bool isDead = false;
    public bool IsDead { get { return isDead; } set { isDead = value; } }
    private bool crosshairStaysActive = false;


    protected Vector3 initialPosition;
    public int Health
    {
        get { return health; }
        protected set
        {
            health = value;
            combatInfo.SetHealth(health);
        }
    }

    protected readonly Dictionary<string, StatusEffect> statusEffects = new Dictionary<string, StatusEffect>();



    protected List<ActionClass> actionsAvailable;
    public DeadEntities _DeathHandler { protected get;  set; }
    public DeadEntities DeathHandler { get; private set; }

#nullable enable

    public delegate void DamageDelegate(int damage);
    public event DamageDelegate? EntityTookDamage;

    public delegate void EntityDelegate(EntityClass player);
    public static event EntityDelegate? OnEntityDeath;
    public static event EntityDelegate? OnEntityClicked;
    public event EntityDelegate? BuffsUpdatedEvent;

    public Sprite? icon;

    public virtual void Start()
    {
        initialPosition = myTransform.position;

        DeEmphasize();
        DisableDice();
        GetComponent<SpriteRenderer>().sortingLayerName = CombatManager.Instance.FADE_SORTING_LAYER;

        _DeathHandler = Die;
        DeathHandler = delegate { return _DeathHandler(); };
        CombatManager.OnGameStateChanging += UpdateBuffsNewRound;
    }

    private void OnDestroy()
    {
        CombatManager.OnGameStateChanging -= UpdateBuffsNewRound;
    }

    /*
     Purpose: Deals damage to this entity and staggers it back 
     Requires: This Entity is not dead
     */

    public virtual void TakeDamage(EntityClass source, int damage)
    {
        BuffsOnDamageEvent(ref damage);

        Health = Mathf.Clamp(Health - damage, 0, MaxHealth);
        float percentageDone = 1; //Testing different powered knockbacks
        if (Health != 0)
        {
            percentageDone = Mathf.Clamp(damage / (float)Health, 0f, 1f);
        } else
        {
            OnEntityDeath?.Invoke(this);
        }

        EntityTookDamage?.Invoke(damage);
        combatInfo.DisplayDamage(damage);
        StartCoroutine(PlayHitAnimation(source, this, percentageDone));
    }

    //Plays both first the stagger entities then 
    //Requires: Entities are not dead
    private IEnumerator PlayHitAnimation(EntityClass origin, EntityClass target, float percentageDone)
    {
        yield return StartCoroutine(StaggerEntities(origin, target, percentageDone));
    }

    /* 
    Purpose: Staggers an entity back 
    origin: The origin of the damage/attack is coming from
    target: The target being staggered back
    percentageDone: Percentage health done to the target
    Requires: Entities are not dead
     */
    public IEnumerator StaggerEntities(EntityClass origin, EntityClass target, float percentageDone)
    {
        Vector3 directionVector = target.myTransform.position - origin.myTransform.position;

        Vector3 normalizedDirection = directionVector.normalized;
        float staggerPower = StaggerPowerCalculation(percentageDone);
        yield return StartCoroutine(target.StaggerBack(target.myTransform.position + normalizedDirection * staggerPower));
    }

    //Calculates the power of the stagger based on the percentage health done
    protected virtual float StaggerPowerCalculation(float percentageDone)
    {
        float minimumPush = 0.8f;
        float pushSlope = 1.8f;
        float percentageUntilMaxPush = 1f / 3f; //Reaches Max push at 33% hp lost
        return minimumPush + pushSlope * Mathf.Clamp(percentageDone / percentageUntilMaxPush, 0f, 1.5f);
    }

    /*
     * Usage:
    Vector3 destination: Destination Of the Moving individual
    float radius: Radius is the radius right before the destination the entity will stop at.
    (Can be useful to prevent two enemies from clipping together)
    float duration: Duration of the movement

    Modifies: this.myTransform

    Purpose: Moves this entity to a given location

    Requires: Entity is not dead
     */
    public virtual IEnumerator MoveToPosition(Vector3 destination, float radius, float duration, Vector3? lookAtPosition = null)
    {
        Vector3 originalPosition = myTransform.position;
        float elapsedTime = 0f;

        Vector3 diffInLocation = destination - originalPosition;
        if ((Vector2)diffInLocation == Vector2.zero) yield break;

        float distance = Mathf.Sqrt(diffInLocation.x * diffInLocation.x + diffInLocation.y * diffInLocation.y);
        float maxProportionTravelled = (distance - radius) / distance;


        if (distance > radius + PLAY_RUNNING_ANIMATION_DELTA)
        {
            UpdateFacing(diffInLocation, lookAtPosition);
            if (HasAnimationParameter("IsMoving"))
            {
                animator.SetBool("IsMoving", true);
            }
        }

        while (elapsedTime < duration)
        {
            myTransform.position = Vector3.Lerp(originalPosition, destination, elapsedTime / duration * maxProportionTravelled);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if (HasAnimationParameter("IsMoving"))
        {
            animator.SetBool("IsMoving", false);
        }
    }
    public void FaceRight()
    {
        FlipTransform(this.transform, true);
        combatInfo.FaceRight();
    }

    public void FaceLeft()
    {
        FlipTransform(this.transform, false);
        combatInfo.FaceLeft();
    }

    public bool IsFacingRight()
    {
        return transform.localScale.x > 0;
    }
    public void FlipTransform(Transform transform, bool faceRight)
    {
        if (faceRight) //Face Right
        {
            Vector3 flippedTransform = transform.localScale;
            flippedTransform.x = Mathf.Abs(flippedTransform.x);
            transform.localScale = flippedTransform;
            if (boxCollider)
            {
                Vector3 currentScale = boxCollider.size;
                boxCollider.size = new Vector3(Mathf.Abs(currentScale.x), Mathf.Abs(currentScale.y), Mathf.Abs(currentScale.z));
            }
        }
        else
        {
            Vector3 flippedTransform = transform.localScale;
            flippedTransform.x = -Mathf.Abs(flippedTransform.x);
            transform.localScale = flippedTransform;
            if (boxCollider)
            {
                Vector3 currentScale = boxCollider.size;
                boxCollider.size = new Vector3(-Mathf.Abs(currentScale.x), Mathf.Abs(currentScale.y), Mathf.Abs(currentScale.z));
            }
        }
    }

    /*
     * Purpose: Updates the entitiy's direction to face a target only when called. (Target.position - my position)
     * diffInLocation: Will face the entity Right if the Target is to its right (positive diffInLocation)
        Left if other way around  
        Note: If you want to reverse the results, add a negative to diffInLocation before calling.
    lookAtPosition: null if you want to use default movement facing, provide a value if you want the player to face a certain direction when called
     */
    public void UpdateFacing(Vector3 diffInLocation, Vector3? lookAtPosition)
    {
        if (lookAtPosition != null)
        {
            diffInLocation = lookAtPosition.Value - myTransform.position;
        }
        if (diffInLocation.x > 0)
        {
            FaceRight();
        }
        else if (diffInLocation.x < 0)
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
     * Requires: Entity is not dead
     */
    public IEnumerator StaggerBack(Vector3 staggeredPosition)
    {
        Vector3 originalPosition = myTransform.position;
        float elapsedTime = 0f;

        Vector3 diffInLocation = staggeredPosition - originalPosition;
        if ((Vector2)diffInLocation == Vector2.zero) yield break;
        UpdateFacing(-diffInLocation, null);

        if (HasAnimationParameter("IsStaggered"))
        {
            animator.SetBool("IsStaggered", true);
        }

        float duration = animator.GetCurrentAnimatorStateInfo(0).length;
        if (duration > CardComparator.COMBAT_BUFFER_TIME) duration = CardComparator.COMBAT_BUFFER_TIME - 0.2f; //Ensure that animation doesn't exceed buffer time or bug will happen with death.

        while (elapsedTime < duration)
        {
            myTransform.position = Vector3.Lerp(originalPosition, staggeredPosition, AnimationCurve(elapsedTime, duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (HasAnimationParameter("IsStaggered"))
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
        Health = Mathf.Clamp(Health + val, 0, MaxHealth);
    }

    public void SetUnstaggered()
    {
        if (HasAnimationParameter("IsStaggered"))
        {
            animator.SetBool("IsStaggered", false);
        }
    }

    public override void OnMouseEnter()
    {
        Highlight();
    }

    public override void OnMouseExit()
    {
        DeHighlight();
    }

    public void CrossHair()
    {
        crosshairStaysActive = true;
        Highlight();
    }

    public void UnCrossHair()
    {
        crosshairStaysActive = false;
        DeHighlight();
    }

    public override void Highlight()
    {
        if (CombatManager.Instance.CanHighlight())
        {
            combatInfo.ActivateCrosshair();
        }
    }

    public override void DeHighlight()
    {
        if (!crosshairStaysActive)
        {
            combatInfo.DeactivateCrosshair();
        }
    }

    public override void OnMouseDown()
    {
        OnEntityClicked?.Invoke(this);
    }
    //Run this to reset the entity position back to its starting position
    public abstract IEnumerator ResetPosition();

    //Removes entity cards and self from BQ and combat manager. Kills itself
    public abstract IEnumerator Die();
    /*
    // Constructor
    public EntityClass(int health)
    {
        this.health = health;
        this.MAX_HEALTH = health;
    } */

    // Checks if a Given Buff exists, instantiates it if not
    private void CheckBuff(string buffType)
    {
        if (!statusEffects.ContainsKey(buffType))
        {
            statusEffects[buffType] = BuffFactory.GetStatusEffect(buffType);
        }
    }

    // Adds the Stacks of the Card to the Relevant Buff Stacks of the Player    
    public virtual void AddStacks(string buffType, int stacks)
    {
        CheckBuff(buffType);
        statusEffects[buffType].GainStacks(stacks);
        UpdateBuffs();
    }

    public void ReduceStacks(string buffType, int stacks)
    {
        if (statusEffects.ContainsKey(buffType))
        {
            statusEffects[buffType].LoseStacks(stacks);
            UpdateBuffs();
        }
    }

    // Applies the Stacks of the Specified Buff to the Card Roll Limits
    private void ApplyBuffsToCard(ref ActionClass.CardDup dup, string buffType)
    {
        CheckBuff(buffType);
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

    // Updates buffs affected by player taking damage
    protected void BuffsOnDamageEvent(ref int damage)
    {
        foreach (string s in statusEffects.Keys)
        {
            statusEffects[s].OnEntityHitHandler(ref damage);
        }
        UpdateBuffs();
    }

    // Updates buffs that change when a new round begins
    private void UpdateBuffsNewRound(GameState newState)
    {
        if (newState == GameState.SELECTION)
        {
            foreach (string s in statusEffects.Keys)
            {
                statusEffects[s].NewRound();
            }
            UpdateBuffs();
        }
    }

    public virtual void AttackAnimation(string animationName)
    {
        if (HasAnimationParameter(animationName))
        {
            animator.SetTrigger(animationName);
        }
    }

    public virtual void BlockAnimation()
    {
        //Todo
        //Implement Block Animation
    }

    public bool HasAnimationParameter(string paramName, Animator? paramAnimator = null)
    {
        if (!paramAnimator)
        {
            paramAnimator = animator;
        }
        foreach (AnimatorControllerParameter param in paramAnimator!.parameters)
        {
            if (param.name == paramName) return true;
        }
        return false;
    }

    public void ActivateCombatInfo(ActionClass actionClass)
    {
        combatInfo.ActivateCombatSprite(actionClass);
    }

    public void DeactivateCombatInfo(ActionClass actionClass)
    {
        combatInfo.DeactivateCombatSprite(actionClass);
    }
    //Increases this Entity Class' sorting layer (negative number is higher up)
    public void Emphasize()
    {
        Vector3 largeTransform = transform.position;
        largeTransform.z = CombatManager.Instance.FADE_SORTING_ORDER - 3;
        transform.position = largeTransform;
        GetComponent<SpriteRenderer>().sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER + 1;
        combatInfo.Emphasize();
    }

    //Decreases this Entity Class' sorting layer. (Standardizes Sorting Layers for entities)
    public void DeEmphasize()
    {
        Vector3 largeTransform = transform.position;
        largeTransform.z = CombatManager.Instance.FADE_SORTING_ORDER - 1;
        transform.position = largeTransform;
        GetComponent<SpriteRenderer>().sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER - 1;
        combatInfo.DeEmphasize();
        
    }

    public void OutOfCombat()
    {
        DisableHealthBar();
        statusEffects.Clear();
        UpdateBuffs();
    }

    public void InCombat()
    {
        EnableHealthBar();
    }

    public void UnTargetable()
    {
        boxCollider.enabled = false;
    }

    public void Targetable()
    {
        boxCollider.enabled = true;
    }

    public void SetDice(int value)
    {
        combatInfo.SetDice(value);
    }

    public void UpdateBuffs()
    {
        combatInfo.UpdateBuffs(statusEffects);
        BuffsUpdatedEvent?.Invoke(this);
    }

    //Please use the originalHandler to resubscribe when you are done :3
    public StatusEffectModifyValueDelegate SetBuffsOnHitHandler(string buff, StatusEffectModifyValueDelegate handler)
    {
        CheckBuff(buff);
        StatusEffectModifyValueDelegate originalHandler = statusEffects[buff].OnEntityHitHandler;
        statusEffects[buff].OnEntityHitHandler = handler;
        Debug.Log("A buff handler is being reassigned, be careful!");
        return originalHandler;
    }

    public StatusEffectModifyValueDelegate GetBuffsOnHitHandler(string buff)
    {
        CheckBuff(buff);
        return statusEffects[buff].OnEntityHitHandler;
    }


    public void EnableDice()
    {
        combatInfo.EnableDice();
    }
    public void DisableDice()
    {
        combatInfo.DisableDice();
    }
    private void EnableHealthBar()
    {
        combatInfo.EnableHealthBar();
    }

    private void DisableHealthBar()
    {
        combatInfo.DisableHealthBar();
    }

    //@Author: Anrui
    //Sets the location an entity returns to after fighting ends.
    public void SetReturnPosition(Vector3 newReturningPosition)
    {
        initialPosition = newReturningPosition;
    }
}
