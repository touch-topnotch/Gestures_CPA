using File = System.IO.File;

namespace Scripts.Network
{
    public class DataChanel
    {
        public static void Send(string jsonPath, string value)
        {
            File.WriteAllText(jsonPath, value);
        }

        public static string Get(string jsonPath)
        {
            return File.ReadAllText(jsonPath);
        }
    }
}