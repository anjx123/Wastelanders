using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityClass : MonoBehaviour
{
    protected int MAX_HEALTH;
    protected int health;
    public int Health { get; set; }

    protected int id;
    public int Id { get; set; }


    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void TakeDamage(int damage)
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
