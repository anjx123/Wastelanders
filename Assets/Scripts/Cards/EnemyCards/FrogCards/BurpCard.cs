using System.Collections.Generic;
using UnityEngine;

namespace Cards.EnemyCards.FrogCards
{
    public class BurpCard : FrogAttacks
    {
        public static readonly List<GameObject> SpawnableEnemies = new List<GameObject>();

        [SerializeField] private ProjectileBehaviour projectileBehaviour;

        public override void Initialize()
        {
            base.Initialize();

            myName = "Burp";
            description = "If not staggered: Spawns a random monster, then this monster loses 2 Resonate.";

            lowerBound = upperBound = 1;
            Speed = 2;
            CardType = CardType.Defense;

            ogMaterial = GetComponent<Renderer>().material;
            OriginalPosition = transform.position;
        }

        public override void CardIsUnstaggered()
        {
            var stacks = Origin.GetBuffStacks(Resonate.buffName);
            if (stacks < 2) return;

            Origin.AttackAnimation("OnSmile");
            Origin.ReduceStacks(Resonate.buffName, 2);

            var projectileDirection = Vector3.down + (Origin.IsFacingRight() ? Vector3.right : Vector3.left);
            var position = Origin.transform.position + projectileDirection;

            StartCoroutine(projectileBehaviour.ProjectileAnimation(OnProjectileHit, Origin, position));
            return;

            void OnProjectileHit()
            {
                var prefab = SpawnableEnemies[Random.Range(0, SpawnableEnemies.Count)];
                var parent = Origin.transform.parent;
                var spawn = Instantiate(prefab, position, Quaternion.identity, parent);
                var entity = spawn.GetComponent<EntityClass>();

                entity.transform.localScale = Vector3.one * (entity is Beetle ? 0.75f : 1);
            }
        }
    }
}