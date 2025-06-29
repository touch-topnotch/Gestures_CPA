using System;
using Scripts.Gestures;
using Scripts.PlayerLogic;
using Scripts.Static.Definitions;
using UnityEngine.Events;

namespace Scripts.Gesture_Editor_SDK.Realtime
{
    public interface IGestureAbility
    {
        public string abilityName { get; }
        public AbilityType type { get; }
        public DynamicGesture dynamicGesture { get; }
        public PlayerData playerData { get; set; }
        public void Initialize(PlayerData data, DynamicGesture gesture);
        public void ReadyToBeRecognized();
        public void OnFrameRecognized(string name);
        public void OnGestureCasted();
        public UnityEvent AbilityReleasedEvent { get; set; }
        public bool TryGetNetcodeId(out ulong id);
    }
}