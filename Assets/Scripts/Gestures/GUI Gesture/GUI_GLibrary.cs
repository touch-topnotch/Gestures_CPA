using Scripts.Hands;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Scripts.Gestures.GGUI
{
    public class DG_Water : GUIGesture
    {
        private GameObject _water0Effect;
        private GameObject _water1Effect;

        protected override void Construct()
        {
            _water0Effect = LoadAsset(Resources.Load("Effects/Water/TestWaterParticle"),
                hands.leftHand.points[3], new Vector3(0, 0, 0));
            _water1Effect = LoadAsset(Resources.Load("Effects/Water/TestWaterRedParticle Variant"),
                hands.rightHand.points[3], new Vector3(0, 0, 0));
            Debug.Log("Water assets added!");
        }

        public override void ShowEffects(int frameId, GestureFrame gFrame)
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

    public class DG_Katana : GUIGesture
    {
        private Transform _katana;

        protected override void Construct()
        {
            _katana = LoadAsset(Resources.Load("Effects/Melee/Katana/KatanaPrefab"),
                hands.rightHand.points[3], new Vector3(0, 0, 0)).transform;
            _katana.gameObject.SetActive(true);
            //_katana.localPosition  = new Vector3(-0.04f,-0.032f,-0.025f);
            _katana.eulerAngles = new Vector3(0, -90, 0);
            Debug.Log("Katana assets added!");
        }

        public override void ShowEffects(int frameId, GestureFrame gFrame)
        {
            switch (frameId)
            {
                case 0:
                    hands.rightHand.SetColorSmooth(HandShaderProps.EdgeColor, Color.cyan, 3);
                    break;
                case 1:
                    hands.rightHand.SetFingersColor(Color.cyan, true);
                    _katana.gameObject.SetActive(true);
                    break;
                case 2:
                
                    break;
            }
        }
    }

    public class DG_Fire : GUIGesture
    {
        private GameObject _sparksEffect;
        private GameObject _largeFlameEffect;

        protected override void Construct()
        {
            _sparksEffect = LoadAsset(Resources.Load("Effects/Fire/SparksEffect"),
                hands.leftHand.points[3], new Vector3(0, 0, 0));
            _largeFlameEffect = LoadAsset(Resources.Load("Effects/Fire/LargeFlameEffect"),
                hands.rightHand.points[3], new Vector3(0, 0, 0));
            Debug.Log("Fire assets added!");
        }

        public override void ShowEffects(int frameId, GestureFrame gFrame)
        {
        }
    }
}