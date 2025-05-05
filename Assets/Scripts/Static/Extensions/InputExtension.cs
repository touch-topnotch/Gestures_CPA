using UnityEngine;

namespace Scripts.Static.Extensions
{
    public static class InputExtension
    {
        public static bool GetKeyWithCtrlOrCmd(KeyCode key) => CtrlOrCmd() && UnityEngine.Input.GetKeyDown(key);
        public static bool CtrlOrCmd()
        {
            return (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
                ? Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)
                : Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        }

    }
}