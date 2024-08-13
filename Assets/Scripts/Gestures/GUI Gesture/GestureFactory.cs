using Scripts.Gestures.GGUI;
using UnityEngine;

namespace Scripts.Gestures
{
    public static class GestureFactory
    {
        public static DynamicGesture SetDynamicGesture(string name)
        {
            DynamicGesture gesture = new DynamicGesture(name);
            switch(name)
            {
                case "Water":
                    gesture.Graphics = new DG_Water(ref gesture.OnFrameDetected);
                    break;
                case "Earth":
                    gesture.Graphics = new DG_Earth(ref gesture.OnFrameDetected);
                    break;
                default:
                    Debug.LogWarning("Couldn't find GUI Gesture for gesture: " + name);
                    break;
                    
            }
            return gesture;
        }
    }
}