using System;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.Events
{
     public class WeaponEvent
        {
            private readonly ushort unityEventId;
            private readonly Action<ushort> callServerRpc;
            private readonly UnityEvent unityEvent;
            private readonly bool isInvokeAvailable;
            public WeaponEvent(UnityEvent unityEvent, Action<ushort> callServerRpc, ushort unityEventId, bool isInvokeAvailable)
            {
                this.unityEvent = unityEvent;
                this.callServerRpc = callServerRpc;
                this.unityEventId = unityEventId;
                this.isInvokeAvailable = isInvokeAvailable;
            }
            public void AddListener(UnityAction a) => unityEvent.AddListener(a);
            public void RemoveListener(UnityAction a) => unityEvent.RemoveListener(a);
            public virtual void Invoke()
            {
                if (isInvokeAvailable)
                {
                    callServerRpc(unityEventId);
                }
                else
                {
                    Debug.LogWarning("No permissions to invoke in this platform! Check Weapon.invokeAvailable");
                }
            }
        }
        public class WeaponEvent<T>
        {
            private readonly ushort unityEventId;
            private readonly Action<T, ushort> callServerRpc;
            private readonly UnityEvent<T> unityEvent;
            private readonly bool isInvokeAvailable;
            public WeaponEvent(UnityEvent<T> unityEvent, Action<T, ushort> callServerRpc, ushort unityEventId, bool isInvokeAvailable)
            {
                this.unityEvent = unityEvent;
                this.callServerRpc = callServerRpc;
                this.unityEventId = unityEventId;
                this.isInvokeAvailable = isInvokeAvailable;
            }
            public void AddListener(UnityAction<T> a) => unityEvent.AddListener(a);
            public void RemoveListener(UnityAction<T> a) => unityEvent.RemoveListener(a);
            public virtual void Invoke(T value)
            {
                if (isInvokeAvailable)
                {
                    callServerRpc(value, unityEventId);
                }
                else
                {
                    Debug.LogWarning("No permissions to invoke in this platform! Check Weapon.invokeAvailable");
                }
               
            }
        }
}