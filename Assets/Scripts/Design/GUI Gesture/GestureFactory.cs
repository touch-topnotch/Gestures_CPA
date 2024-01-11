using Scripts.Gestures.GGUI;
using UnityEngine;

namespace Scripts.Gestures
{
    public static class GestureFactory
    {
        public static GUIGesture SetDynamicGesture(string name)
        {
            switch(name)
            {
                case "Katana":
                    return new DG_Katana();
                default:
                    Debug.LogWarning("Couldn't find GUI class or its implementation for gesture: " + name + ". If it exists, please override Gesture Factory");
                    break;
            }
            return null;
        }
    }
}