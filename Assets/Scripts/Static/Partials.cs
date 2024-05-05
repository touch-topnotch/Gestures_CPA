using UnityEngine;

namespace Scripts.Static
{
    public static class Partials
    {
        public static bool GetKeyWithCtrlOrCmd( this Input input, KeyCode key)
        {
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) || (Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)))
            {
                if (Input.GetKeyDown(key))
                {
                    return true;
                }
            }
            return false;
        }
    }
}