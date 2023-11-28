using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionClass : MonoBehaviour
{
    //The following are 'properties' in C# that make quick getters and setters for private fields. ac.Target for access
    public EntityClass Target { get; set; }
    public EntityClass Origin { get; set; }

    private int damage;
    private int block;
    private int speed;

    public abstract void ExecuteActionEffect();



}
