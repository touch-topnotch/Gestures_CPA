using Scripts.Gestures;
using Scripts.HandsLogic;
using Scripts.PlayerLogic;
using UnityEngine;
using UnityEngine.Events;

namespace Gesture_Editor_SDK.Realtime
{
    public abstract class RecognizableObject : MonoBehaviour, IRecognizable
    {

        private UnityEvent _onAbilityReleased = new UnityEvent();
        public PlayerData playerData { get; set; }
        public abstract void OnFrameRecognized(string name);

        public abstract void AbilityCalled();
        protected abstract void OnAbilityReleased();
    
        public void AbilityReleased()
        {
            OnAbilityReleased();
            _onAbilityReleased?.Invoke();
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

        UnityEvent IRecognizable.OnAbilityReleased => _onAbilityReleased;
    }
    //recognizable object ALWAYS should know information about player.
}
