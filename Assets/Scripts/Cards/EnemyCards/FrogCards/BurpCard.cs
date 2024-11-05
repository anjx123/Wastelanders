using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cards.EnemyCards.FrogCards
{
    public class BurpCard : FrogAttacks
    {
        public static readonly List<GameObject> SpawnableEnemies = new List<GameObject>();

        public override void Initialize()
        {
            base.Initialize();

            myName = "Burp";
            description = "On Hit: Lose 2 Resonate, if so, spawn a random monster.";

            lowerBound = upperBound = 1;
            Speed = 2;
            CardType = CardType.RangedAttack;

            ogMaterial = GetComponent<Renderer>().material;
            OriginalPosition = transform.position;
        }

        protected override void OnProjectileHit()
        {
            base.OnProjectileHit();
            var stacks = Origin.GetBuffStacks(Resonate.buffName);
            if (stacks < 2) return;
            Origin.ReduceStacks(Resonate.buffName, 2);

            var projectileDirection = Vector3.down + (Origin.IsFacingRight() ? Vector3.left : Vector3.right);
            var position = Target.transform.position + projectileDirection;


            var prefab = SpawnableEnemies[Random.Range(0, SpawnableEnemies.Count)];
            var parent = Origin.transform.parent;
            var spawn = Instantiate(prefab, position, Quaternion.identity, parent);
            var entity = spawn.GetComponent<EntityClass>();

            entity.transform.localScale = Vector3.one * (entity is Beetle ? 0.75f : 1);
        }
    }
}