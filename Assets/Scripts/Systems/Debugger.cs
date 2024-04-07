using System.Collections.Generic;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace Scripts.Systems
{
    public static class Debugger
    {
        public static string listToString<T>(in List<T> array, in bool inLine = false)
        {
            string line = "";
            foreach (T t in array)
            {
                line +=inLine ? ", " : "\n";
                line += t.ToString();
            }

            return line;
        }
        public static string arrayToString<T>(in T[] array, in bool inLine = false)
        {
            string line = "";
            foreach (T t in array)
            {
                line +=inLine ? ", " : "\n";
                line += t.ToString();
            }

            return line;
        }
    }
}