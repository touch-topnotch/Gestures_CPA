using Scripts.Components;
using Scripts.Gesture_Editor_SDK.Realtime;
using Scripts.Gestures;
using Scripts.PlayerLogic;
using Scripts.Players;
using Scripts.Static.Definitions;
using UnityEngine.Events;

namespace Gesture_Editor_SDK.Realtime
{
    public abstract class NetworkRecognizableComponent : NetworkInheritedComponent<Player>, IGestureAbility
    {
        private UnityEvent _abilityReleasedEvent;

        public abstract string abilityName { get; }
        public abstract AbilityType type { get; }
        public DynamicGesture dynamicGesture => _gesture;
        public PlayerData playerData { get; set; }
        private DynamicGesture _gesture;

        public virtual void Initialize(PlayerData data, DynamicGesture gesture)
        {
            this.playerData = data;
            this._gesture = gesture;
        }
        
        public abstract void ReadyToBeRecognized();

        public abstract void OnFrameRecognized(string name);

        public abstract void OnGestureCasted();

        public abstract UnityEvent AbilityReleasedEvent { get; set; }

        public bool TryGetNetcodeId(out ulong id)
        {
            id = NetworkObject.NetworkObjectId;
            return true;
        } 
    }
}