using Director;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SceneBuilder
{
    public class DefaultSceneBuilder : SceneBuilder
    {
        protected override void Build()
        {
            DefaultDirector.Build();

        }
    }
}
namespace Director
{
    public interface Director { }

    public class DefaultDirector : MonoBehaviour, Director
    {
        public static DefaultDirector Build()
        {
            GameObject director = new GameObject(typeof(DefaultDirector).Name);
            return director.AddComponent<DefaultDirector>();
        }

        public void OnEnable()
        {
            EntityClass.OnEntitySpawn += HandleSpawns;
        }

        public void OnDisable()
        {

            EntityClass.OnEntitySpawn -= HandleSpawns;
        }

        public void HandleSpawns(EntityClass entityClass)
        {
            if (entityClass.Team == EntityClass.EntityTeam.PlayerTeam)
            {

            }
        }
    }
}
