using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Systems.Persistence
{
    [Serializable] public class GameData
    {
        public string Name;
        public List<ActionData> ActionData;
    }

    public interface ISaveable
    {
        SerializableGuid Id { get; set; }
    }

    public interface IBind<TData> where TData : ISaveable 
    {
        SerializableGuid Id { get; set; }
        void Bind(TData data);
    }

    // Singleton that shows up in unity
    public class SaveLoadSystem : PersistentSingleton<SaveLoadSystem> {
        [SerializeField] public GameData GameData { get; private set; }

        IDataService dataService;

        protected override void Awake()
        {
            base.Awake();
            dataService = new FileDataService(new JSonSerializer());
        }

        public void LoadCardEvolutionProgress()
        {
            Bind<ActionClass, ActionData>(GameData.ActionData);
        }

        void Bind<T, TData>(List<TData> datas) where T: MonoBehaviour, IBind<TData> where TData : ISaveable, new() {
            T[] entities = FindObjectsByType<T>(FindObjectsSortMode.None);

            foreach (T entity in entities)
            {
                TData data = datas.FirstOrDefault(it => it.Id == entity.Id);
                if (data == null)
                {
                    data = new TData { Id = entity.Id };
                    datas.Add(data);
                }
                entity.Bind(data);
            }
        }

        void Bind<T, TData>(TData data) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            T entity = FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();

            if (entity != null)
            {
                if (data == null)
                {
                    data = new TData { Id = entity.Id };
                }
                entity.Bind(data);
            }
        }

        public void NewGame()
        {
            GameData = new GameData
            {
                Name = "Wastelanders Save File",
            };
        }

        public void SaveGame()
        {
            dataService.Save(GameData);
        }

        public void LoadGame(string gameName)
        {
            GameData = dataService.Load(gameName);
        }
    }
}