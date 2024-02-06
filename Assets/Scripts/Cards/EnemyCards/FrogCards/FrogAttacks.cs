using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;
public abstract class FrogAttacks : ActionClass
{
    private delegate IEnumerator TakeDamageDelegate(EntityClass source, int damage);
    [SerializeField]
    private GameObject spitPrefab;



    public override void Initialize()
    {
        base.Initialize();
        CardType = CardType.RangedAttack;
    }

    public override IEnumerator OnHit()
    {
        Vector3 diffInLocation = Target.myTransform.position - Origin.myTransform.position;
        Origin.UpdateFacing(diffInLocation, null);
        Origin.AttackAnimation("IsShooting");
        if (spitPrefab != null)
        {
            yield return StartCoroutine(ProjectileAnimation(this.Target.TakeDamage, Origin, Target, duplicateCard.actualRoll));
        }
    }

    private IEnumerator ProjectileAnimation(TakeDamageDelegate takeDamageCallback, EntityClass origin, EntityClass target, int damage)
    {
        yield return StartCoroutine(StartProjectileAnimation(origin, target));
        yield return StartCoroutine(takeDamageCallback(origin, damage));
    }

    public IEnumerator StartProjectileAnimation(EntityClass origin, EntityClass target)
    {
        Vector3 originalPosition = origin.myTransform.position;
        Vector3 destination = target.myTransform.position;
        float elapsedTime = 0f;

        Vector3 diffInLocation = destination - originalPosition;

        if ((Vector2)diffInLocation == Vector2.zero) yield break;

        float distance = Mathf.Sqrt(diffInLocation.x * diffInLocation.x + diffInLocation.y * diffInLocation.y);

        //UpdateFacing(diffInLocation, destination);
        GameObject spitProjectile = Instantiate(spitPrefab, originalPosition, UpdateAngle(origin, target));
        SpriteRenderer spitSpriteRenderer = spitProjectile.GetComponent<SpriteRenderer>();
        spitSpriteRenderer.sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER;
        spitSpriteRenderer.sortingLayerName = CombatManager.Instance.FADE_SORTING_LAYER;
        float flipTimer = 0.0f;
        float flipInterval = 0.2f;
        float spitDuration = distance / 10f;

        while (elapsedTime < spitDuration)
        {
            spitProjectile.GetComponent<Transform>().position = Vector3.Lerp(originalPosition, destination, elapsedTime / spitDuration);
            elapsedTime += Time.deltaTime;
            flipTimer += Time.deltaTime;

            if (flipTimer >= flipInterval)
            {
                spitSpriteRenderer.flipY = !spitSpriteRenderer.flipY;
                flipTimer = 0.0f;
            }
            yield return null;
        }

        Destroy(spitProjectile);
    }

    private Quaternion UpdateAngle(EntityClass origin, EntityClass target)
    {
        Vector3 direction = target.myTransform.position - origin.myTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(new Vector3(0, 0, angle));
    }
}