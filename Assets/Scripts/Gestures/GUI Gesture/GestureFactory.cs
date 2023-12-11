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
                case "Katana":
                    gesture.graphics = new DG_Katana();
                    break;
                default:
                    Debug.LogWarning("Couldn't find GUI class or its implementation for gesture: " + name + ". If it exists, please override Gesture Factory");
                    break;
            }
            return gesture;
        }
    }
}