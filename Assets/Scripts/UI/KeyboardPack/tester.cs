using UnityEngine;

namespace UI.KeyboardPack
{
    public class tester : MonoBehaviour

    {
        public void StartEdit()
        {
            Debug.Log("Start editing text");
        }

        public void DebugChanged(string text)
        {
            Debug.Log("Input field changed on " + text);
        }

        public void Exit(string text)
        {
            Debug.Log("End editing with " + text);
        }
    }
}