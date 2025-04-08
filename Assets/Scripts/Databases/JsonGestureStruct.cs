using System.Collections.Generic;

namespace Scripts.Databases
{
    public struct JsonGestureStruct
    {
        public string key;
        public JsonGestureProperty value;
    }

    public struct JsonGestureProperty
    {
        public int Type;
        public List<string[]> Frames;
    }
    // на данном этапе мы рассматриваем только ресурсы, находящиеся в папке Resources, соответсвенно делаем с рут обьектом
}