using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class Crystals : EnemyClass, NeutralEntityInterface
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        MaxHealth = 15;
        Health = MaxHealth;
        myName = "Le Shiny";
    }

    public override void TakeDamage(EntityClass source, int damage)
    {
        int oldHealth = Health; // This ensures the buff is added only once when each criteria is met
        base.TakeDamage(source, damage);
        
        // obtains how many damage thresholds crossed
        int numThresholds = ((Health <= 0 && oldHealth > 0) ? 1 : 0) + ((Health <= 5 && oldHealth > 5) ? 1 : 0) + ((Health <= 10 && oldHealth > 10) ? 1 : 0);
        source.AddStacks(Resonate.buffName, numThresholds);
    }

    public override void AddAttack(List<PlayerClass> players) {}

    public override IEnumerator MoveToPosition(Vector3 destination, float radius, float duration, Vector3? lookAtPosition = null)
    {
        Vector3 originalPosition = myTransform.position;

        Vector3 diffInLocation = destination - originalPosition;

        if ((Vector2)diffInLocation == Vector2.zero) yield break;

        UpdateFacing(diffInLocation, lookAtPosition);
        yield break;
    }

    // @author: andrew
    // i NEEDED to move crystals
    public IEnumerator CrystalMoveToPosition(Vector3 destination, float radius, float duration, Vector3? lookAtPosition = null)
    {
        yield return StartCoroutine(base.MoveToPosition(destination, radius, duration));
    }
}
