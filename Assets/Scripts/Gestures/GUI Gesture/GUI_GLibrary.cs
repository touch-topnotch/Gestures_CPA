using Scripts.Hands;
using UnityEngine;
using Zenject;

namespace Scripts.Gestures.GGUI
{

    public class DG_Water: GUIGesture
    {
        private GameObject _water0Effect;
        private GameObject _water1Effect;
        
        public DG_Water(ref FrameDetected onFrameDetected) : base(ref onFrameDetected)
        {
            
        }
        public override void Construct(UserHands hands)
        {
            _water0Effect = LoadAsset(Resources.Load("Effects/Water/TestWaterParticle"),
                hands.LeftSkeleton.GetTransforms()[0], new Vector3(0, 0, 0));
            _water1Effect = LoadAsset(Resources.Load("Effects/Water/TestWaterRedParticle Variant"),
                hands.LeftSkeleton.GetTransforms()[0], new Vector3(0, 0, 0));
            Debug.Log("Water assets added!");
        }

        protected override void ShowEffects(int frameId, GestureFrame gFrame)
        {
            switch (frameId)
            {
                case 0:
                    _water0Effect.SetActive(true);
                    break;
                case 1:
                    _water1Effect.SetActive(true);
                    break;
                    
            }
        }

     
    }
    public class DG_Fire: GUIGesture
    {
        public DG_Fire(ref FrameDetected onFrameDetected) : base(ref onFrameDetected)
        {
        }

        protected override void ShowEffects(int frameId, GestureFrame frame)
        {
            switch (frameId)
            {
                case 0:
                    Frame0(frame);
                    break;
            }
        }

        private void Frame0(GestureFrame frame)
        {   
            Debug.Log("Типо спавню огонь в указательном пальце левой руки: " + frame.Hands.LeftPoints[0]);
        }

    }
}