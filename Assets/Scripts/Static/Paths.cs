

using System;
using System.Text;

namespace Scripts.Static
{
    public struct CustomPaths
    {
        public const string CharacterManager = "Assets/Prefabs/Managers/CharacterController.prefab";
        public const string Characters = "Assets/Resources/Characters";
        public const string Weapons = "Assets/Resources/Weapons";
        public static string CharacterNameFolder(string name) => Characters + "/" + name;
        public const string TelegramResources = "Assets/TelegramResources";
        public const string InNested = "@UnityEditor.PrefabUtility.IsPartOfPrefabInstance(gameObject)";
        public const string projectId = "7ebfa65c-a5a4-45c0-8a74-343f849aac1c";
        public const string environmentId = "e788f0fc-30d5-4cff-9fda-4726d919687d";
        public const string keyId = "fa5b5a42-d8eb-438c-92fc-2e350b45063d";
        public const string keySecret = "Fbey4vc1XydQfW8TTnU3gsl1aYBTYSlO";
        public static string keyBase64 => Convert.ToBase64String(Encoding.UTF8.GetBytes(keyId + ":" + keySecret));
 
    }
}