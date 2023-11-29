using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityClass : MonoBehaviour
{
    protected int MAX_HEALTH;
    protected int health;
    protected Material outliner;
    protected Material ogMaterial;
    protected bool isOutlined = false; // deals with enemy selection
    public int Health
    {
        get { return health; }
        set { health = value; }
    }

    protected int id;
    public int Id 
    { 
        get { return id; }
        set { id = value; }
    }

    public void OnMouseEnter() 
    {
        if (!isOutlined) {
            Renderer renderer = GetComponent<Renderer>();
            renderer.material = outliner;
        }
    }

    public void OnMouseExit()
    {
        if (!isOutlined) {
            Renderer renderer = GetComponent<Renderer>();
            renderer.material = ogMaterial;
        }
    }

    public void OnMouseDown()
    {
        TakeDamage(2);
        FrogHighlightManager.OnFrogClicked(this);
    }

    public void Toggle()
    {
        isOutlined = !isOutlined;

        if (isOutlined) {
            Highlight();
        } else {
            DeHighlight();
        }
    }

    public void Highlight()
    {
        Renderer renderer = GetComponent<Renderer>();
        isOutlined = true;
        renderer.material = outliner;
    }

    public void DeHighlight() 
    {
        Renderer renderer = GetComponent<Renderer>();
        isOutlined = false;
        renderer.material = ogMaterial;
    }


    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public virtual void TakeDamage(int damage)
    {
        health = Mathf.Clamp(health - damage, 0, MAX_HEALTH);
        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Entity: " + id + " has died");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
