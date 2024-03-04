using System.Collections.Generic;

namespace Scripts.Databases
{
    public struct JsonCharacterStruct
    {
        public string Name;
        public string Description;
        public string RootFolder;
        public List<JsonGestureStruct> Gestures;
    }
}