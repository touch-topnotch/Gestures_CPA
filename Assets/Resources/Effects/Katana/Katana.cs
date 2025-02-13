using System;
using Gesture_Editor_SDK.Realtime;
using Scripts.Design;
using Scripts.Effects;
using Scripts.Gestures;
using Scripts.HandsLogic;
using UnityEngine;

    public class Katana : RecognizableObject
    {
        
        [SerializeField] private DissolveSlider _glow;
        [SerializeField] private DissolveSlider _blade;
        [SerializeField] private DissolveSlider _handle;
        [SerializeField] private GameObject _particles;
        [SerializeField] private GameObject _materialization;

        private PlayerHands _hands => playerData.hands;
        private void Awake()
        {
           
            _blade.gameObject.SetActive(false);
            _handle.gameObject.SetActive(false);
            _glow.gameObject.SetActive(false);
            _particles.gameObject.SetActive(false);
            _materialization.gameObject.SetActive(false);
        }

        private void Start()
        {
            ChangeParent(transform, _hands.rightHand.points[0]);
        }

        public override void OnFrameRecognized(string frameName)
        {
            
            var frameId = GestureMapper.IndexOfName(frameName);
            switch (frameId)
            {
                case 0:
                    _handle.gameObject.SetActive(true);
                    _hands.rightHand.SetColorSmooth(HandShaderProps.MainColor, new Color(0.1f, 0, 0.2f, 0.55f), 3);
                    _hands.leftHand.SetColorSmooth(HandShaderProps.MainColor, new Color(0.1f, 0, 0.2f, 0.55f), 3);
                    _hands.rightHand.SetColorSmooth(HandShaderProps.EdgeColor, new Color(0.53f, 0, 0.8f, 0), 3);
                    _hands.leftHand.SetColorSmooth(HandShaderProps.EdgeColor, new Color(0.53f, 0, 0.8f, 0), 3);
                    _handle.UpdateDisolveValue(0f);
                    break;
                case 1:
                    _hands.rightHand.SetColorSmooth(HandShaderProps.EdgeColor, new Color(0.53f, 0, 0.8f), 3);
                    _hands.leftHand.SetColorSmooth(HandShaderProps.EdgeColor, new Color(0.53f, 0, 0.8f), 3);
                    _handle.UpdateDisolveValue(1f);
                    break;
                
                case 2:
                    _blade.gameObject.SetActive(true);
                    _blade.UpdateDisolveValue(0f);
                    break;
                 
                case 3:
                    _blade.UpdateDisolveValue(1f);
                    _materialization.gameObject.SetActive(true);
                    break;
                
                case 4:
                    _glow.UpdateDisolveValue(0f);
                    _glow.gameObject.SetActive(true);
                    break;
                
                case 5:
                    _glow.UpdateDisolveValue(1f);
                    _particles.SetActive(true);
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


