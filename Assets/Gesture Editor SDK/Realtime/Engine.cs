using Scripts.Hands;
using Scripts.PlayerLogic;
using Zenject;

namespace Gesture_Editor_SDK.Realtime
{
    public class Engine
    {
        public EngineStats stats;
        private static Engine _instance;

        // Private constructor to prevent creating instances of the class
        private Engine()
        {
        }

        // Public method to get the singleton instance
        public static Engine Instance()
        {
            if (_instance == null)
            {
                _instance = new Engine();
            }
            return _instance;
        }
    }

    public struct EngineStats
    {
        public PlayerHands hands;
        public BodyAnchors bodyAnchors;
    }

}