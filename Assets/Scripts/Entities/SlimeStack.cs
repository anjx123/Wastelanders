using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class SlimeStack : EnemyClass
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        MaxHealth = 15;
        Health = MaxHealth;
        myName = "Le Slime Stack";
    }

    public void UnTargetable()
    {
        boxCollider.enabled = false;
    }

    public void Targetable()
    {
        boxCollider.enabled = true;
    }

}
