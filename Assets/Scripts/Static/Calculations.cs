using System;
using System.Linq;

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
    }
}