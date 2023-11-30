using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityClass : AllClass
{
    protected int MAX_HEALTH;
    protected int health;
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


    public override void OnMouseDown()
    {
        TakeDamage(2);
        HighlightManager.OnEntityClicked(this);
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
