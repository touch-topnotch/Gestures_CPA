using Scripts.Gestures;
using UnityEngine.Events;

namespace Scripts.Events
{
    public class RecognitionEvent : UnityEvent<DynamicGesture>
    {

    }

    public delegate void FrameDetected(int frameId, GestureFrame frame);
}