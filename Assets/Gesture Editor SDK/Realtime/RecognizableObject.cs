using System;
using Scripts.Gesture_Editor_SDK.Realtime;
using Scripts.Gestures;
using Scripts.PlayerLogic;
using Scripts.Static.Definitions;
using UnityEngine;
using UnityEngine.Events;

namespace Gesture_Editor_SDK.Realtime
{

    public abstract class RecognizableBehaviour : MonoBehaviour, IGestureAbility
    {
        public string abilityName => transform.name;
        public abstract AbilityType type { get; }
        public DynamicGesture dynamicGesture => _dynamicGesture;
        public PlayerData playerData { get; set; }
        public abstract void Initialize();

        private DynamicGesture _dynamicGesture;
        public void Initialize(PlayerData data, DynamicGesture gesture)
        {
            playerData = data;
            _dynamicGesture = gesture;
        }

        public abstract void ReadyToBeRecognized();
        
        public abstract void OnFrameRecognized(string name);

        public abstract void OnGestureCasted();
      
        protected abstract void OnAbilityReleased();
        
        public UnityEvent AbilityReleasedEvent { get; set; }
        public bool TryGetNetcodeId(out ulong id)
        {
            id = 0;
            return false;
        }

        protected void ChangeParent(Transform obj, Transform parent, bool adjustTransform = true)
        {
            obj.SetParent(parent);
            if (adjustTransform)
            {
                obj.localPosition = Vector3.zero;
                obj.rotation = Quaternion.identity;
            }
        }
    }
    //recognizable object ALWAYS should know information about player.
}