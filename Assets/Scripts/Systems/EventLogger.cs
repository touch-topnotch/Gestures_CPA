using Scripts.PlayerLogic;
using UnityEngine;

namespace Scripts.Systems
{
    public class EventLogger
    {
        public static void OnPlayerModeChanger(PlayerMode mode)
        {
            Debug.Log("Player mode changed on " + mode);
        }
    }
}