
using Scripts.Gestures;
using UnityEngine.Events;

namespace Gesture_Editor_SDK.Realtime
{
    public interface IRecognizable
    {
        void OnFrameRecognized(int frameId, GestureFrame frame);
        void AbilityCalled();
        void AbilityReleased();
        
        UnityEvent OnAbilityReleased { get; }
    }
}
