using System.Collections.Generic;
using Newtonsoft.Json;
using Scripts.Databases;
using Scripts.Network;
using UnityEngine.Device;

namespace Characters
{
    public static class CharacterMapper
    {
        private static List<JsonCharacterStruct> jsonStruct;
        private static string characterLibrary =>
            DataChanel.Get(Application.dataPath + "/Resources/Database/CharacterLibrary.json");

        public static List<JsonCharacterStruct> GetCharacterStruct()
        {
            if(jsonStruct == null)
                jsonStruct = JsonConvert.DeserializeObject<List<JsonCharacterStruct>>(characterLibrary);
            return jsonStruct;
        }

        public static void SendCharacterStruct(List<JsonCharacterStruct> characterStruct)
        {
            jsonStruct = characterStruct;
            DataChanel.Send(Application.dataPath + "/Resources/Database/CharacterLibrary.json",
                JsonConvert.SerializeObject(characterStruct, Formatting.Indented));
        }

    }
}