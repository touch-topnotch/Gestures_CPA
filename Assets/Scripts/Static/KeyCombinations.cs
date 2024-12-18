using UnityEngine;

namespace Scripts.Static
{
    public static class KeyCombinations
    {
        public static bool ControlCommand(KeyCode code) =>
            (Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(code);

    }
}