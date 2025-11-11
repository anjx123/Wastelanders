using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Managers
{
    public class ImpactFrameManager : MonoBehaviour
    {
        [SerializeField] private float impactTime = 0.2f;
        public static ImpactFrameManager Instance { get; private set; }
        
        public bool IsFrozen { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != null && Instance != this)
            {
                Destroy(this);
            }

            this.AddComponent<OnHitEventHandler>()
                .Subscribe(HandleOnHit);
        }
        
        private class OnHitEventHandler : EventHandler<OnHitEvent> { }

        private void HandleOnHit(OnHitEvent e)
        {
            Debug.Log("[ImpactFrameManager] OnHitEvent");
            StartCoroutine(FreezeForSeconds(e.Source, impactTime));
        }
        
        private IEnumerator FreezeForSeconds(ActionClass source, float duration) {
            if (IsFrozen) yield break;
            IsFrozen = true;
            
            Animator animator1 = source.Origin.animator;
            Animator animator2 = source.Target.animator;
            
            animator1.speed = 0f;
            animator2.speed = 0f;

            yield return new WaitForSecondsRealtime(duration);

            animator1.speed = 1f;
            animator2.speed = 1f;
            
            IsFrozen = false;
        }
    }
}

public record OnHitEvent(ActionClass Source, OnHitEnum Type) : IEvent;

public enum OnHitEnum
{
    None = 0,
    PlayerHitEnemy = 1,
    PlayerHitObject = 2,
    EnemyHitPlayer = 3,
    EnemyHitObject = 4,
    Other = 5
}