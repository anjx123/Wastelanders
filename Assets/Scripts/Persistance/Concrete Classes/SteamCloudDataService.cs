using System.IO;
using Steamworks;
using UnityEngine;

namespace Systems.Persistence {
    public class SteamCloudDataService : IDataService {
        private readonly ISerializer serializer;
        private const string EXTENSION = ".json";

        public SteamCloudDataService(ISerializer serializer) {
            this.serializer = serializer;
        }

        private string GetPath(string name) => name + EXTENSION;

        public void Save(GameData data, bool overwrite = true) {
            if (!SteamManager.Initialized) {
                Debug.LogWarning("[SteamCloudDataService] Steam is not initialized. Cannot save.");
                return;
            }

            string json = serializer.Serialize(data);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
            string path = GetPath(data.Name);

            bool result = SteamRemoteStorage.FileWrite(path, bytes, bytes.Length);
            Debug.Log($"[SteamCloudDataService] Saved to Steam Cloud: {path} ({result})");
        }

        public GameData Load(string name) {
            if (!SteamManager.Initialized) {
                throw new IOException("Steam is not initialized.");
            }

            string path = GetPath(name);
            if (!SteamRemoteStorage.FileExists(path)) {
                throw new IOException($"Steam Cloud file '{path}' does not exist.");
            }

            int size = SteamRemoteStorage.GetFileSize(path);
            byte[] buffer = new byte[size];

            int read = SteamRemoteStorage.FileRead(path, buffer, size);
            string json = System.Text.Encoding.UTF8.GetString(buffer, 0, read);

            return serializer.Deserialize<GameData>(json);
        }
    }
}