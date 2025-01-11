using System;
using System.Linq;
using Unity.VisualScripting;

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
    }
}