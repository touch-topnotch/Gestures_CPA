using CrossPlatform.Gestures;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace CrossPlatform.PlayerLogic
{
    public class CrossPlatformPlayer:Player
    {
        public XRInputModalityManager inputManager;
        public HandSkeleton leftHand;
        public HandSkeleton rightHand;
        public PointsHandGenerator pointsHandGen;
        public override void Initialize()
        { 
            InitializeHands();
        }
        public override Vector3[] GetLeftHandPoints() => leftHand.GetBones();

        public override Vector3[] GetRightHandPoints() => rightHand.GetBones();

     
        private void InitializeHands()
        {
            if (!leftHand)
                leftHand = transform.Find("Left Hand").GetComponent<HandSkeleton>();
            if (!rightHand)
                rightHand = transform.Find("Right Hand").GetComponent<HandSkeleton>();
            
            inputManager.trackedHandModeStarted.AddListener(HandEnabled);
            inputManager.trackedHandModeStarted.AddListener(leftHand.HandEnabled);
            inputManager.trackedHandModeStarted.AddListener(rightHand.HandEnabled);
            inputManager.trackedHandModeStarted.AddListener(pointsHandGen.Initialize);
            
            inputManager.trackedHandModeEnded.AddListener(HandDisabled);
            inputManager.trackedHandModeEnded.AddListener(leftHand.HandDisabled);
            inputManager.trackedHandModeEnded.AddListener(rightHand.HandDisabled);
            
        }
        
        private void HandEnabled(){}
        private void HandDisabled(){}
    }
}