using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using File = System.IO.File;
using Scripts.Static;
using Telegram.Bot.Types.Enums;

namespace Scripts.Network
{
    public static class DataChanel
    {
     
        public static void WriteAndSendFile(string filePath, string value)
        {
          
            Debug.Log("Trying to write");
            Task.Run(async () =>
            {
                await WriteAndSendFileAsync(filePath, value);
            });
            
        }


        private static async Task WriteAndSendFileAsync(string filePath, string value)
        {
            await TelegramBotProcessor.SendTextToTelegram("```" + value + "```");
            // using (StreamWriter writer = new StreamWriter(filePath))
            // {
            //     await writer.WriteAsync(value);
            // }
            //
            // var name = Path.GetFileName(filePath);
            //
            // await TelegramBotProcessor.SendFileToTelegram(name, value);
            
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