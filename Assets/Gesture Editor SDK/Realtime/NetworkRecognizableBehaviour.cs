using System;
using Scripts.PlayerLogic;
using Unity.Netcode;
using UnityEngine;

namespace Gesture_Editor_SDK.Realtime
{
    public abstract class NetworkRecognizableBehaviour : NetworkBehaviour, IRecognizable
    {
        public PlayerData playerData { get; set; }
        public abstract void OnFrameRecognized(string name);

        public abstract void AbilityCalled();
        protected abstract void OnAbilityReleased();
    
        public void AbilityReleased()
        {
            OnAbilityReleased();
            onAbilityReleased?.Invoke();
            // do functions and destroy it;
        }

        public event Action onAbilityReleased;
    }
}