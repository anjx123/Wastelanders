using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
#nullable enable
    public delegate void OnHitDelegate();
    [SerializeField]
    private GameObject? projectilePrefab;

    public IEnumerator ProjectileAnimation(OnHitDelegate onHitCallback, EntityClass origin, EntityClass target)
    {
        return ProjectileAnimation(onHitCallback, origin, target.myTransform.position);
    }

    public IEnumerator ProjectileAnimation(OnHitDelegate onHitCallback, EntityClass origin, Vector3 targetPosition)
    {
        yield return StartCoroutine(StartProjectileAnimationWithPosition(origin, targetPosition));
        onHitCallback();
    }

    private IEnumerator StartProjectileAnimationWithPosition(EntityClass origin, Vector3 targetPosition)
    {
        if (projectilePrefab == null) yield break;
        Vector3 originalPosition = origin.myTransform.position;
        float elapsedTime = 0f;

        Vector3 diffInLocation = targetPosition - originalPosition;

        if ((Vector2)diffInLocation == Vector2.zero) yield break;

        float distance = Mathf.Sqrt(diffInLocation.x * diffInLocation.x + diffInLocation.y * diffInLocation.y);

        //UpdateFacing(diffInLocation, destination);
        GameObject spitProjectile = Instantiate(projectilePrefab, originalPosition, UpdateAngleWithPosition(origin, targetPosition));
        SpriteRenderer spitSpriteRenderer = spitProjectile.GetComponent<SpriteRenderer>();
        spitSpriteRenderer.sortingOrder = CombatManager.Instance.FADE_SORTING_ORDER;
        spitSpriteRenderer.sortingLayerName = CombatManager.Instance.FADE_SORTING_LAYER;
        float flipTimer = 0.0f;
        float flipInterval = 0.2f;
        float spitDuration = distance / 10f;

        while (elapsedTime < spitDuration)
        {
            spitProjectile.GetComponent<Transform>().position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / spitDuration);
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

    private Quaternion UpdateAngleWithPosition(EntityClass origin, Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - origin.myTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
