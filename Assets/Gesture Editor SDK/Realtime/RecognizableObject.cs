using Scripts.Gestures;
using UnityEngine;
using UnityEngine.Events;

namespace Gesture_Editor_SDK.Realtime
{
    public abstract class RecognizableObject : MonoBehaviour, IRecognizable
    {
        private UnityEvent _onAbilityReleased = new UnityEvent();
        public abstract void OnFrameRecognized(int frameId, GestureFrame frame);

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
}
