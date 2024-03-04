using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using File = System.IO.File;
using Scripts.Static;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using UnityEngine.Networking.PlayerConnection;

namespace Scripts.Network
{
    public static class DataChanel
    {
     
        public static void Send(string jsonPath, string value)
        {
            
            File.WriteAllText(jsonPath, value);
            SendDocumentAsync(jsonPath);
        }
        static async void SendDocumentAsync(string jsonPath)
        {
            try
            {
                await TelegramBotProcessor.SendToTelegramAsync(jsonPath);
                Debug.Log("Document sent successfully.");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to send document: {e.Message}");
            }
        }

        public static string Get(string jsonPath)
        {
            jsonPath = Calculations.ConvertToResourceFormat(jsonPath);
            var jsonFile = Resources.Load<TextAsset>(jsonPath);
            if (jsonFile != null) 
                return jsonFile.text;
            Debug.LogError("Failed to load JSON file from resources: " + jsonPath);
            return "";
        }
        public static string Get(string jsonPath, ulong id)
        {
            return "";
        }
    
    }
}