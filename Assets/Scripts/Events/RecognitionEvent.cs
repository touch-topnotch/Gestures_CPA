using Scripts.Gestures;
using UnityEngine.Events;

namespace Scripts.Events
{
    public class RecognitionEvent : UnityEvent<DynamicGesture>
    {

    }

    public class FrameDetected : UnityEvent<int, GestureFrame>{}
//frameId, GestureFrame frame);
}