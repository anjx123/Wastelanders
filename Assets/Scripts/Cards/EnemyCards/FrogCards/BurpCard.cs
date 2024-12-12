using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cards.EnemyCards.FrogCards
{
    public class BurpCard : FrogAttacks, IPlayablePrincessFrogCard
    {
        [SerializeField] private List<GameObject> SerializedSpawnableEnemies = new List<GameObject>();
        public static readonly List<GameObject> SpawnableEnemies = new List<GameObject>();

        public override void Initialize()
        {
            base.Initialize();

            myName = "Burp";
            description = "On Hit: Lose 2 Resonate, if so, spawn a random monster.";

            CostToAddToDeck = 2;
            lowerBound = upperBound = 1;
            Speed = 2;
            CardType = CardType.RangedAttack;
        }

        protected override void OnProjectileHit()
        {
            base.OnProjectileHit();
            var stacks = Origin.GetBuffStacks(Resonate.buffName);
            if (stacks < 2) return;
            Origin.ReduceStacks(Resonate.buffName, 2);

            var projectileDirection = Vector3.down + (Origin.IsFacingRight() ? Vector3.left : Vector3.right);
            var position = Target.transform.position + projectileDirection;


            var prefab = GetAppropriateSpawningList()[Random.Range(0, GetAppropriateSpawningList().Count)];
            var parent = Origin.transform.parent;
            var spawn = Instantiate(prefab, position, Quaternion.identity, parent);
            var entity = spawn.GetComponent<EntityClass>();
            entity.Team = Origin.Team;

            entity.transform.localScale = Vector3.one * (entity is Beetle ? Beetle.BEETLE_SCALING : 1);
        }

        private List<GameObject> GetAppropriateSpawningList()
        {
            return SerializedSpawnableEnemies.Count != 0 ? SerializedSpawnableEnemies : SpawnableEnemies;
        }
    }
}