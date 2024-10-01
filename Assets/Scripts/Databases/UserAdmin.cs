using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Scripts.Network;
using Scripts.Static;

namespace Scripts.Databases
{
    public static class UserAdmin
    {
        private const string databasePath =
            //  "/Users/dmitry057/Projects/UnityProjects/Gestures_CPA/Assets/Resources/Database/UserLibrary.json";
            "C:/Unity Projects/Gestures_CPA/Assets/Resources/Database/UserLibrary.json";
        public static void Add(UserData userData)
        {
            // add userData to json by databasePath 
            List<UserData> userLibrary = JsonConvert.DeserializeObject<List<UserData>>(DataChanel.Get(databasePath));
            if (userLibrary.Count > 1)
                userData.id = userLibrary[-1].id + 1;
            else
                userData.id = 0;
            userLibrary.Add(userData);
            l.rl("UserAdmin.Add"+ "userLibrary.Count = " + userLibrary.Count);
            DataChanel.Send(databasePath, JsonConvert.SerializeObject(userLibrary));
        }

        public static void Override(in UserData userData)
        {
            List<UserData> userLibrary = JsonConvert.DeserializeObject<List<UserData>>(DataChanel.Get(databasePath));
            for (int i = 0; i < userLibrary.Count; i++)
            {
                if (userLibrary[i].id == userData.id)
                {
                    userLibrary[i] = userData;
                } 
            }
            JsonConvert.SerializeObject(userLibrary);
        }

        public static void Remove(in int id)
        {
            List<UserData> userLibrary = JsonConvert.DeserializeObject<List<UserData>>(DataChanel.Get(databasePath));
            for (int i = 0; i < userLibrary.Count; i++)
            {
                if (userLibrary[i].id == id)
                {
                    userLibrary.RemoveAt(i);
                    break;
                } 
            }
        }

        public static bool HasIncluded(in int id)
        {
            List<UserData> userLibrary = JsonConvert.DeserializeObject<List<UserData>>(DataChanel.Get(databasePath));
            for (int i = 0; i < userLibrary.Count; i++)
            {
                if (userLibrary[i].id == id)
                {
                    return true;
                } 
            }

            return false;
        }
    }
}