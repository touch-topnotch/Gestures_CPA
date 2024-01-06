using UnityEngine;

namespace Scripts.PlayerLogic
{
    [CreateAssetMenu(fileName="PlayerData_", menuName = "Config/PlayerData")]
    public class PlayerData: ScriptableObject
    {
        public readonly float health;
    }
}