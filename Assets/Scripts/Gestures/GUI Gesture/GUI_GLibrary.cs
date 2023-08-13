using Scripts.Events;
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
                hands.RightSkeleton.GetTransforms()[3], new Vector3(0, 0, 0));
            _water1Effect = LoadAsset(Resources.Load("Effects/Water/TestWaterRedParticle Variant"),
                hands.RightSkeleton.GetTransforms()[3], new Vector3(0, 0, 0));
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

    public class DG_Earth : GUIGesture
    {
        public DG_Earth(ref FrameDetected onFrameDetected) : base(ref onFrameDetected)
        {
        }

        protected override void ShowEffects(int frameId, GestureFrame gFrame)
        {
            
        }
    }
}