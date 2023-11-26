using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WasteFrog : EntityClass
{
    List<ActionClass> availableActions;
    public Animator animator;
    public Transform myTransform;

    // Start is called before the first frame update
    void Start()
    {
        MAX_HEALTH = 15;
        health = MAX_HEALTH;
        animator = GetComponent<Animator>();
        myTransform = GetComponent<Transform>();
    }

    public void OnMouseDown()
    {
        TakeDamage(2);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        StartCoroutine(StaggerBack(myTransform.position + new Vector3(1f, 0, 0)));
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
    IEnumerator StaggerBack(Vector3 staggeredPosition)
    {
        Vector3 originalPosition = myTransform.position;
        float elapsedTime = 0f;
        
        animator.SetBool("IsStaggered", true);
        float duration = animator.GetCurrentAnimatorStateInfo(0).length;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            float speed = duration / AnimationCurve(elapsedTime, duration); // This will cause the speed to decay over time
            float lerpScaler = AnimationCurve(duration, duration);

            myTransform.position = Vector3.Lerp(myTransform.position, staggeredPosition, t * speed * lerpScaler);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        animator.SetBool("IsStaggered", false);
    }

    private float AnimationCurve(float elapsedTime, float duration)
    {
        float delta = 0.000001f; //To prevent divide by 0
        return 1f / (elapsedTime / Mathf.Pow(elapsedTime + delta, (7f / 10f)));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
