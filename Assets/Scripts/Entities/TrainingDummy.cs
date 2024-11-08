using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingDummy : EnemyClass
{

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        MaxHealth = 10;
        Health = MaxHealth;
        myName = "Le Dummy";
    }

    public override IEnumerator Die()
    {
        BattleQueue.BattleQueueInstance.RemoveAllInstancesOfEntity(this);
        CombatManager.Instance.RemoveEnemy(this);
        yield break;
    }

    protected override float StaggerPowerCalculation(float percentageDone)
    {
        float minimumPush = 0f;
        float pushSlope = 1f;
        float staggerMultiplier = 1f;

        float percentageUntilMaxPush = 1f / 2f; //Reaches Max push at 33% hp lost
        return minimumPush + pushSlope * Mathf.Clamp(percentageDone / percentageUntilMaxPush, 0f, staggerMultiplier);
    }

    public override IEnumerator MoveToPosition(Vector3 destination, float radius, float duration, Vector3? lookAtPosition = null)
    {
        Vector3 originalPosition = myTransform.position;

        Vector3 diffInLocation = destination - originalPosition;

        if ((Vector2)diffInLocation == Vector2.zero) yield break;

        UpdateFacing(diffInLocation, lookAtPosition);
        yield break;
    }
}
