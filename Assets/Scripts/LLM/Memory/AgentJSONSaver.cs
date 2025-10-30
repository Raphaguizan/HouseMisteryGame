using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace Guizan.LLM
{
    public enum SavePathFolder
    {
        npc_Memory,
        embedding
    }
    public class AgentJSONSaver<T>
    {
        private static string Folder;

        private  static void SetMemoryFolder(SavePathFolder folder)
        {
            Folder = Path.Combine(Application.persistentDataPath, folder.ToString());
            //Debug.Log(Folder);
        }
        /// <summary>
        /// Carrega JSON.
        /// </summary>
        public static T LoadJSON(string npcId, SavePathFolder pathType = SavePathFolder.npc_Memory, string playerId = "")
        {
            SetMemoryFolder(pathType);
            string path = GetPath(npcId, playerId);
            if (!File.Exists(path)) return default;

            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json) ?? default;
        }

        /// <summary>
        /// Salva dados em JSON.
        /// </summary>
        public static void SaveJSON(string npcId, T data, SavePathFolder pathType = SavePathFolder.npc_Memory, string playerId = "")
        {
            SetMemoryFolder(pathType);
            string path = GetPath(npcId, playerId);
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        public static void ClearJSON(string npcId, SavePathFolder pathType = SavePathFolder.npc_Memory, string playerId = "")
        {
            SetMemoryFolder(pathType);
            string path = GetPath(npcId, playerId);
            if (File.Exists(path))
                File.Delete(path);
        }

        private static string GetPath(string npcId, string playerId = "")
        {
            string fileName = $"{playerId}_{npcId}.json";
            return Path.Combine(Folder, fileName);
        }
    }
}