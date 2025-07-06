using System.Collections.Generic;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace Scripts.Systems
{
    public static class Debugger
    {
        public static string listToString<T>(in List<T> array, in bool inLine = false)
        {
            if (array == null) return "null";
            string line = "";
            bool was = false;
            foreach (T t in array)
            {
                if(was)
                    line += inLine ? ", " : "\n";
                was = true;
                line += t.ToString();
            }

            return line;
        }

        public static string dictionaryToString<K, V>(in Dictionary<K, V> array, in bool showValues = false,
            in bool inLine = false)
        {
            if (array == null) return "null";
            string line = "";
            bool was = false;
            foreach (var t in array)
            {
                if(was)
                    line += inLine ? ", " : "\n";
                was = true;
                if (showValues)
                {
                    line += t.Key.ToString() + " - " + t.Value?.ToString();
                }
                else
                {
                    line += t.Key;
                }
            }

            return line;
        }

        public static string arrayToString<T>(in T[] array, in bool inLine = false)
        {
            if (array == null) return "null";
            string line = "";
            bool was = false;
            foreach (T t in array)
            {
                if(was)
                    line += inLine ? ", " : "\n";
                was = true;
                line += t.ToString();
            }

            return line;
        }
    }
}