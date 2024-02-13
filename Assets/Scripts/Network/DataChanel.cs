using UnityEngine;
using File = System.IO.File;
using Scripts.Static;

namespace Scripts.Network
{
    public static class DataChanel
    {
        public static void Send(string jsonPath, string value)
        {
            
            File.WriteAllText(jsonPath, value);
        }

        public static string Get(string jsonPath, ulong id)
        {
            jsonPath = Calculations.ConvertToResourceFormat(jsonPath);
            var jsonFile = Resources.Load<TextAsset>(jsonPath);
            if (jsonFile != null) 
                return jsonFile.text;
            Debug.LogError("Failed to load JSON file from resources: " + jsonPath);
            return "";
        }
    }
}