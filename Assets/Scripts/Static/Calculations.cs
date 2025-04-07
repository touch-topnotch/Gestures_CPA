using System;
using System.Linq;
using Scripts.Network;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

namespace Scripts.Static
{
    public static class Calculations
    {
        private static Random random = new Random();

        public static string RandomString(int length, string prefix = "")
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return prefix + new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static bool fEqual(float a, float b, float quality = 0.001f) =>
            Math.Abs(a - b) < quality;

        public static string ConvertToResourceFormat(string path)
        {

            // Find the index of the "Resources" keyword in the original path
            int resourcesIndex = path.IndexOf("Resources", StringComparison.Ordinal);

            if (resourcesIndex != -1)
            {
                // Extract the substring from the original path, starting from the index of "Resources" + 9 (length of "Resources/")
                string subPath = path.Substring(resourcesIndex + 10);

                // Remove the file extension
                subPath = subPath.Substring(0, subPath.LastIndexOf('.'));

                return subPath;
            }
            else
            {
                // If the "Resources" keyword is not found in the original path
                throw new ArgumentException("The original path does not contain the keyword 'Resources'.");
            }
        }
        public static T AddComponentSmart<T>(Transform transf)
        where T : Component
        {
            if (transf.TryGetComponent<T>(out var temp))
                return temp;
            return transf.AddComponent<T>();
        }

        public static Vector3 Rotate(Vector3 vec, float yAngle)
        {
            float radian = yAngle * Mathf.Deg2Rad;
            float newX = vec.x * Mathf.Cos(radian) - vec.z * Mathf.Sin(radian);
            float newZ = vec.x * Mathf.Sin(radian) + vec.z * Mathf.Cos(radian);
    
            return new Vector3(newX, vec.y, newZ);
        }
    }
}