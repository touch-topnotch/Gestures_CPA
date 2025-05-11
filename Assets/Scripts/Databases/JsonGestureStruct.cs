using System.Collections.Generic;

namespace Scripts.Databases
{
    public struct JsonFrameStruct
    {
        public string key;
        public JsonFrameProperty value;
    }

    public struct JsonFrameProperty
    {
        public int Type;
        public List<string[]> Frames;
    }
    // на данном этапе мы рассматриваем только ресурсы, находящиеся в папке Resources, соответсвенно делаем с рут обьектом
}