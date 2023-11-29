using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WasteFrog : EntityClass
{
    private string myName = "Le Frog";
    List<ActionClass> availableActions;
    public Animator animator;
    public Transform myTransform;
    public Material outliner;
    public Material ogMaterial;
    private bool isOutlined;

    // Start is called before the first frame update
    void Start()
    {
        MAX_HEALTH = 15;
        health = MAX_HEALTH;
        animator = GetComponent<Animator>();
        myTransform = GetComponent<Transform>();
        Debug.Log(myName + " is ready for combat!");
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of frog
        
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        // StartCoroutine(StaggerBack(myTransform.position + new Vector3(1.5f, 0, 0)));
    }

        public void OnMouseDown()
    {
        TakeDamage(2);

        isOutlined = !isOutlined;
        ToggleOutline();
    }

    void ToggleOutline()
    {
        Renderer renderer = GetComponent<Renderer>();
        Material currentMaterial = isOutlined ? outliner : ogMaterial;

        if (isOutlined)
        {
            currentMaterial.SetColor("_OutlineColor", Color.yellow);
        }

        renderer.material = currentMaterial;
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
            myTransform.position = Vector3.Lerp(myTransform.position, staggeredPosition, AnimationCurve(elapsedTime, duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        animator.SetBool("IsStaggered", false);
    }

    private float AnimationCurve(float elapsedTime, float duration)
    {
        float speed = 1f; //Lower value is faster
        float power = 5f; //Modifies the curvature of the curve
        return (Mathf.Pow(speed, power) / Mathf.Pow(((-elapsedTime)/ duration - speed), power) + 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
