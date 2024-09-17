
using System.IO;
using UnityEngine;

namespace Systems.Persistence
{
    public class FileDataService : IDataService
    {
        ISerializer serializer;
        string dataPath;
        string fileExtension;

        public FileDataService(ISerializer serializer)
        {
            // Application.persistentDataPath = C:\Users\YOUR_USER_NAME\AppData\LocalLow\S2dio\WastelandersGame
            this.dataPath = Application.persistentDataPath;
            this.fileExtension = "json";
            this.serializer = serializer;
        }

        string GetPathToFile(string fileName)
        {
            return Path.Combine(dataPath, string.Concat(fileName, ".", fileExtension));
        }

        public void Save(GameData data, bool overwrite = true)
        {
            string fileLocation = GetPathToFile(data.Name);

            if (!overwrite && File.Exists(fileLocation))
            {
                throw new IOException($"File '{data.Name}.{fileExtension}' already exists and can't be overwritten.");
            }

            File.WriteAllText(fileLocation, serializer.Serialize(data));
        }

        public GameData Load(string name)
        {
            string fileLocation = GetPathToFile(name);

            if (!File.Exists(fileLocation))
            {
                throw new IOException($"File '{name}.{fileExtension}' does not exist in this file");
            }

            return serializer.Deserialize<GameData>(File.ReadAllText(fileLocation));
        }
    }
}