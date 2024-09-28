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
            inputManager.trackedHandModeStarted.AddListener(bodyAnchors.Hands.HandEnabled);
            inputManager.trackedHandModeEnded.AddListener(bodyAnchors.Hands.HandDisabled);
        }
    }
}