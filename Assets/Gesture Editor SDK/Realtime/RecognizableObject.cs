using System;
using Scripts.PlayerLogic;
using UnityEngine;

namespace Gesture_Editor_SDK.Realtime
{
    public abstract class RecognizableBehaviour : MonoBehaviour, IRecognizable
    {
        public string gestureName => this.transform.name;
        public PlayerData playerData { get; set; }
        public abstract void OnFrameRecognized(string name);

        public abstract void AbilityCalled();
        protected abstract void OnAbilityReleased();
    
        public void AbilityReleased()
        {
            OnAbilityReleased();
            AbilityReleasedEvent?.Invoke();
            // do functions and destroy it;
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

        public event Action AbilityReleasedEvent;
    }
    //recognizable object ALWAYS should know information about player.
}
