using System.Collections.Generic;

namespace Scripts.Databases
{
    public struct JsonGestureStruct
    {
        public List<List<string>> Frames;
        public JsonAsset Asset;
    }
    // на данном этапе мы рассматриваем только ресурсы, находящиеся в папке Resources, соответсвенно делаем с рут обьектом

    public struct JsonGUI
    {
        public List<JsonAsset> Assets;
    }

    public struct JsonAsset
    {
        public int Type; //  = static, object
        public string Path;
    }
}