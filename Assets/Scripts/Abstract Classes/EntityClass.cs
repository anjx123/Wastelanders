using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityClass : SelectClass
{
    protected int MAX_HEALTH;
    protected int health;
    public HealthBar healthBar;
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

    public virtual void Start()
    {
        healthBar.setMaxHealth(MAX_HEALTH);
        healthBar.setHealth(MAX_HEALTH);
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

    public virtual void Heal(int val)
    {
        health = Mathf.Clamp(health + val, 0, MAX_HEALTH);
        healthBar.setHealth(health);
    }

    public override void OnMouseDown()
    {
        Heal(1);
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
}
