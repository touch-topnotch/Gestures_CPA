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
            foreach (T t in array)
            {
                line += inLine ? ", " : "\n";
                line += t.ToString();
            }

            return line;
        }

        public static string dictionaryToString<K, V>(in Dictionary<K, V> array, in bool showValues = false,
            in bool inLine = false)
        {
            if (array == null) return "null";
            string line = "";
            bool once = true;
            foreach (var t in array)
            {
                if (!once)
                {
                    line += inLine ? ", " : "\n";
                }
                else
                {
                    once = false;
                }

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
            foreach (T t in array)
            {
                line += inLine ? ", " : "\n";
                line += t.ToString();
            }

            return line;
        }
    }
}