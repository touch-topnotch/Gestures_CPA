using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Scripts.Databases
{
    public struct JsonCharacterStruct
    {
        public string key;
        public JsonCharacterProperties value;

        public JsonCharacterStruct(string key, JsonCharacterProperties value)
        {
            this.key = key;
            this.value = value;
        }
    }


    public struct JsonCharacterProperties
    {
        public string Description;
        public string RootFolder;
        public List<JsonGestureStruct> Gestures;

        public JsonCharacterProperties(string description, string rootFolder, List<JsonGestureStruct> gestures)
        {
            Description = description;
            RootFolder = rootFolder;
            Gestures = gestures;
        }
    }
}