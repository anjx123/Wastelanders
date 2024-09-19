using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Systems.Persistence
{
    [Serializable] public class GameData
    {
        public string Name;
        public List<ActionData> ActionData;
        public GameStateData gameStateData;
        public PlayerInformation playerInformation;
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

    // Singleton save load manager that shows up in unity
    public class SaveLoadSystem : PersistentSingleton<SaveLoadSystem>
    {
        private const string SAVE_FILE_NAME = "Wastelanders Save File";
        [SerializeField] public GameData gameData;
        private PlayerDatabase defaultPlayerDatabase;

        IDataService dataService;

        protected override void Awake()
        {
            base.Awake();
            dataService = new FileDataService(new JSonSerializer());
            defaultPlayerDatabase = Resources.LoadAll<PlayerDatabase>("").First(); // Could consider loading by name for better performance
            try
            {
                LoadGame();
            } catch (IOException e)
            {
                Debug.Log(e.Message + " Starting a new game.");
                NewGame();
                SaveGame();
            }
            LoadPlayerInformation();
            LoadGameStateInformation();
        }

        public void LoadCardEvolutionProgress()
        {
            Bind<ActionClass, ActionData>(gameData.ActionData);
        }

        // Scriptable objects are not saved and loaded like MonoBehaviours are. 
        private void LoadPlayerInformation()
        {
            defaultPlayerDatabase.Bind(gameData.playerInformation);
        }

        public void LoadGameStateInformation()
        {
            Bind<GameStateManager, GameStateData>(gameData.gameStateData);
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
            gameData = new GameData
            {
                Name = SAVE_FILE_NAME,
                gameStateData = new GameStateData(),
                ActionData = new List<ActionData>(),
                playerInformation = new PlayerInformation(defaultPlayerDatabase.JackieData, defaultPlayerDatabase.IvesData) // Initialize with default scriptableObject values
            };
        }

        public void SaveGame()
        {
            Debug.Log("Saving the game!");
            dataService.Save(gameData);
        }

        public void LoadGame()
        {
            gameData = dataService.Load(SAVE_FILE_NAME);
        }
    }
}