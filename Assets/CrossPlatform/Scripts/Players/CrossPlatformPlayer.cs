using CrossPlatform.Gestures;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace CrossPlatform.PlayerLogic
{
    public class CrossPlatformPlayer:Player
    {
        public XRInteractionGroup leftHand;
        public XRInteractionGroup rightHand;
        
        public XRInputModalityManager inputModalityManager;
        private bool _isHandsEnabled = false;
        public override void Initialize()
        {
            inputModalityManager.trackedHandModeStarted.AddListener(OnHandEnabled);
            inputModalityManager.trackedHandModeEnded.AddListener(OnHandDisabled);
        }
        public override Vector3[] GetLeftHandPoints()
        {
            return null;
        }

        public override Vector3[] GetRightHandPoints()
        {
            return null;
            
        }
        

        
        private void OnHandEnabled()
        {
            _isHandsEnabled = true;
        }
        private void OnHandDisabled()
        {
            _isHandsEnabled = false;
        }
    }
}