using UnityEngine;
using UnityEngine.Events;

namespace Scripts.Events
{
    public class UpdateEvent : UnityEvent
    {
        private static UpdateEvent _instance;
        public static UpdateEvent Instance => _instance ??= new UpdateEvent();
    }

    public class UpdateEvent<T> : UnityEvent<T>
    {
    }
}