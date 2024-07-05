using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Systems.Persistence
{
    [Serializable] public class GameData
    {
        public string Name;
    }

    public interface ISaveable
    {
        SerializableGuid Id { get; set; }
    }

    // Singleton that shows up in unity
    public class SaveLoadSystem : PersistentSingleton<SaveLoadSystem> {
        [SerializeField] public GameData gameData;

        IDataService dataService;

        protected override void Awake()
        {
            base.Awake();
            dataService = new FileDataService(new JSonSerializer());
        }

        public void NewGame()
        {
            gameData = new GameData
            {
                Name = "Wastelanders Save File",
            };
        }

        public void SaveGame()
        {
            dataService.Save(gameData);
        }

        public void LoadGame(string gameName)
        {
            gameData = dataService.Load(gameName);
        }
    }
}