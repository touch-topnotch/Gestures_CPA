using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Scripts.Databases;
using Scripts.Network;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using UnityEngine;
using Application = UnityEngine.Device.Application;

namespace Characters
{
    public static class CharacterMapper
    {
        private static string characterLibrary =>
            DataChanel.Get(Application.dataPath + "/Resources/Database/CharacterLibrary.json");

    
        public static async Task<Dictionary<string, JsonCharacterProperties>> GetAvailableCharactersStruct(HashSet<string> characterNames)
        {
            
            var data = await CloudSaveService.Instance.Data.Custom.LoadAsync("characters", characterNames );
     
            if (data == null)
                return null;
            var converted = new Dictionary<string, JsonCharacterProperties>();
            foreach (var charKey in data.Keys)
            {
                if(data[charKey].Value.GetAsString() == "" || data[charKey].Value.GetAsString() == "null")
                    continue;
                converted.Add(charKey, data[charKey].Value.GetAs<JsonCharacterProperties>());
            }

            return converted;
        }

        private static void logData(Dictionary<string, Item> items)
        {
            Debug.Log("Data:");
            if (items == null || items.Keys.Count == 0)
            {
                Debug.Log("NULL");
                return;
            }
            foreach (var key in items.Keys)
            {
                Debug.Log($"{key}, {items[key]}");
            }
        }
        public static async Task<Dictionary<string, JsonCharacterProperties>> GetCharacterStructs()
        {
            var data = await CloudSaveService.Instance.Data.Custom.LoadAllAsync("characters");
            var converted = new Dictionary<string, JsonCharacterProperties>();
            foreach (var charKey in data.Keys)
            {
                if(data[charKey].Value.GetAsString() == "" || data[charKey].Value.GetAsString() == "null")
                    continue;
                Debug.LogWarning(data[charKey].Value.GetAsString());
                converted.Add(charKey, data[charKey].Value.GetAs<JsonCharacterProperties>());
            }

            return converted;
        }
        public static void SendCharacterStruct(JsonCharacterStruct characterStruct)
        {
            CloudSaveProcessor.SetItemToCloud(JsonConvert.SerializeObject(characterStruct), "characters", (e) =>
            {
                Debug.Log("Character " + characterStruct.key + " was sent to cloud");
            });
        }

    }
}