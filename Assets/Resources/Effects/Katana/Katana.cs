using Gesture_Editor_SDK.Realtime;
using Scripts.Effects;
using Scripts.Gestures;
using Scripts.Hands;
using UnityEngine;

    public class Katana : RecognizableObject
    {
        private GameObject _katana;
        private DissolveSlider _dissolveSlider;
        private PlayerHands _hands => Engine.Instance().stats.hands;
        private void Start()
        {
            _katana = transform.GetChild(0).gameObject;
            ChangeParent(transform, _hands.rightHand.points[0]);
            _dissolveSlider = _katana.GetComponent<DissolveSlider>();
            _katana.SetActive(false);
        }

        public override void OnFrameRecognized(string frameName)
        {
            var frameId = GestureMapper.IndexOfName(frameName);
            switch (frameId)
            {
                case 0:
                    _hands.rightHand.SetColorSmooth(HandShaderProps.EdgeColor, Color.cyan, 3);
                    break;
                
                case 1:
                    _katana.gameObject.SetActive(true);
                    _hands.rightHand.SetFingersColor(Color.cyan, true);
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
                    break;
            }
        }

        public override void AbilityCalled()
        {
        
        }

        protected override void OnAbilityReleased()
        {
        
        }
    }


