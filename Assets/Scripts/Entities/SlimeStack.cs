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

    public override void TakeDamage(EntityClass source, int damage)
    {
        MusicManager.Instance.PlaySFX(MusicManager.SFXList.slime_damage_taken);
        base.TakeDamage(source, damage);
    }

}
