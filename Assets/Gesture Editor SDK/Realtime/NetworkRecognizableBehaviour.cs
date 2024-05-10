using System;
using Scripts.Components;
using Scripts.PlayerLogic;
using Unity.Netcode;
using UnityEngine;

namespace Gesture_Editor_SDK.Realtime
{
    public abstract class NetworkRecognizableComponent : NetworkInheritedComponent<Player>, IRecognizable
    {
        public string gestureName => transform.name;
        public PlayerData playerData => inherited.data;
        public abstract void OnFrameRecognized(string name);

        public abstract void AbilityCalled();
        protected abstract void OnAbilityReleased();
    
        public void AbilityReleased()
        {
            OnAbilityReleased();
            AbilityReleasedEvent?.Invoke();
            // do functions and destroy it;
        }

        public event Action AbilityReleasedEvent;
    }
}