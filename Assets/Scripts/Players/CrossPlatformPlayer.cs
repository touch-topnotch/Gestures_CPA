using Scripts.Gestures;
using Scripts.Hands;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace Scripts.PlayerLogic
{
    public class CrossPlatformPlayer: Player
    {
        public XRInputModalityManager inputManager;
        public override void Initialize()
        { 
            base.Initialize();
            InitializeHands();
        }

     
        private void InitializeHands()
        {
            inputManager.trackedHandModeStarted.AddListener(HandEnabled);
            inputManager.trackedHandModeEnded.AddListener(HandDisabled);
        }
        
        private void HandEnabled(){}
        private void HandDisabled(){}
    }
}