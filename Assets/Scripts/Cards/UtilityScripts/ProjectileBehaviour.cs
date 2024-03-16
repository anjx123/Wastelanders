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
        yield return StartCoroutine(StartProjectileAnimation(origin, target));
        onHitCallback();
    }

    private IEnumerator StartProjectileAnimation(EntityClass origin, EntityClass target)
    {
        if (projectilePrefab == null) yield break;
        Vector3 originalPosition = origin.myTransform.position;
        Vector3 destination = target.myTransform.position;
        float elapsedTime = 0f;

        Vector3 diffInLocation = destination - originalPosition;

        if ((Vector2)diffInLocation == Vector2.zero) yield break;

        float distance = Mathf.Sqrt(diffInLocation.x * diffInLocation.x + diffInLocation.y * diffInLocation.y);

        //UpdateFacing(diffInLocation, destination);
        GameObject spitProjectile = Instantiate(projectilePrefab, originalPosition, UpdateAngle(origin, target));
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
