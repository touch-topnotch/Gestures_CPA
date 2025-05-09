using Scripts.Gestures;
using UnityEngine.Events;

namespace Scripts.Events
{
    public class GestureRecognized : UnityEvent<string> // name of Dynamic Gesture
    {
    }

    public class FrameRecognized : UnityEvent<string> // name of Gesture (easy to send between platforms)
    {
    }
}