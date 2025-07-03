using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace Scripts.XR
{
    public class CustomXRInteractionManager: XRInteractionManager
    {
        // create an Initialize() method, which repeats functionality of the base class
        public void Initialize()
        {
            base.Awake();
        }
    }
}