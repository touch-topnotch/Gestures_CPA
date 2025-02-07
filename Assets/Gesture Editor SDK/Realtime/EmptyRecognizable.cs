using Scripts.Gestures;
using UnityEngine;
using UnityEngine.Events;

namespace Gesture_Editor_SDK.Realtime
{
    public class EmptyRecognizable: MonoBehaviour, IRecognizable
    {
        public EmptyRecognizable()
        {
            OnAbilityReleased = new UnityEvent();
        }
        public void OnFrameRecognized(string name)
        {
            Debug.Log("Executed empty gesture on frame: " + name);
        }

        public void AbilityCalled()
        {
            AbilityReleased();
        }

        public void AbilityReleased()
        {
            OnAbilityReleased?.Invoke();
        }

        public UnityEvent OnAbilityReleased { get; }
    }
}