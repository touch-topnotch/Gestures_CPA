using Scripts.PlayerLogic;
using Scripts.Static.Definitions;
using UnityEngine;

namespace Scripts.Systems
{
    public class EventLogger
    {
        public static void OnPlayerModeChanged(PlayerMode mode)
        {
            Debug.Log("Player mode changed on " + mode);
        }
        public static void OnCharacterChanged(string mode)
        {
            Debug.Log("Character changed on " + mode);
        }
    }
}