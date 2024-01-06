using UnityEngine;
using UnityEngine.Events;

namespace Scripts.Events
{
    public class UpdateEvent : UnityEvent{}
    public class UpdateEvent<T> : UnityEvent<T>{}
}