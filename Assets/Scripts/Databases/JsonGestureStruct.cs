using System.Collections.Generic;
using Scripts.Gestures;

namespace Scripts.Databases
{
    public struct JsonGestureStruct
    {
        public GestureType Type;
        public List<List<string>> Frames;
        public JsonGUI GUI;
    }

    public struct JsonGUI
    {
        public List<JsonAsset> Assets;
        public List<string> FrameLogic;
    }

    public struct JsonAsset
    {
        public string Type;
        public string Path;
        public string SpawnPoint;
    }
}