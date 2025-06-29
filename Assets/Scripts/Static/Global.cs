using Scripts.Events;
using Scripts.Network;

namespace Scripts.Static
{
    public static class Global
    {
        private static MonoSingleton<EventManager> _eventManager = new MonoSingleton<EventManager>(true);
        private static MonoSingleton<CloudSaveProcessor> _cloudSaveProcessor = new MonoSingleton<CloudSaveProcessor>(true);
        public static EventManager eventManager => _eventManager.Instance;
        public static CloudSaveProcessor cloudSaveProcessor => _cloudSaveProcessor.Instance;
        public static UpdateEvent updateEvent => eventManager.updateEvent;

    }
}