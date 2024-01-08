using System.Timers;
using Scripts.Events;
using Scripts.Hands;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Zenject;
using Timer = Scripts.Static.Timer;

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
        [Inject] private UpdateEvent onUpdate;
        private Transform _katana;
        private DissolveSlider _dissolveSlider;

        protected override void Construct()
        {
            _katana = LoadAsset(Resources.Load("Effects/Melee/Katana/KatanaPrefab"),
                hands.rightHand.points[0], new Vector3(-0.0391f,-0.034f,0.0696f)).transform;
            _dissolveSlider = _katana.gameObject.GetComponent<DissolveSlider>();
            
            _katana.gameObject.SetActive(true);
            _katana.localPosition  = new Vector3(-0.0391f,-0.034f,0.0696f);
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
                    _dissolveSlider.UpdateDisolveValue(0.8f);
                    break;
                case 2:
      
                    break;
                case 3:
                    _dissolveSlider.UpdateDisolveValue(0.7f);
                    break;
                case 4:
                    break;
                case 5:
                    _dissolveSlider.UpdateDisolveValue(0f);
                    new Timer(5, () =>
                    {
                        _dissolveSlider.UpdateDisolveValue(0f);
                        
                    }, onUpdate);
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