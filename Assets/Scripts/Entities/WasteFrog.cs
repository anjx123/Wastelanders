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

    /* Purpose: Plays when the enemy is staggered
     * Pushes the enemy back by staggeredPosition
     * Plays the stagger animation
     * 
     * Params:
     * staggeredPosition is the location we want the enemy to stop, 
     * 
     * Modifies: myTransform.position
     */
    IEnumerator StaggerBack(Vector3 staggeredPosition)
    {
        Vector3 originalPosition = myTransform.position;
        float speed = 2f; //Conrols the strength of the stagger
        float startTime = Time.time;
        
        animator.SetBool("IsStaggered", true);
        float duration = animator.GetCurrentAnimatorStateInfo(0).length;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            myTransform.position = Vector3.Lerp(myTransform.position, staggeredPosition, t * speed);
            speed *= 0.9f; // This will cause the speed to decay over time
            yield return null;
        }

        animator.SetBool("IsStaggered", false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
