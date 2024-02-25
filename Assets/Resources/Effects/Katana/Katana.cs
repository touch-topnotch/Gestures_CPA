using Scripts.Design;
using Scripts.Effects;
using Scripts.Gestures;
using Scripts.HandsLogic;
using Scripts.Weapons;
using UnityEngine;

    public class Katana : Melee
    {
        [Header("Katana components")]
        [SerializeField] private DissolveSlider _o_glow;
        [SerializeField] private DissolveSlider _o_blade;
        [SerializeField] private DissolveSlider _o_handle;
        [SerializeField] private GameObject _o_particles;
        [SerializeField] private GameObject _o_materialization;
        private PlayerHands _hands => playerData.hands;
        private void Awake()
        {
           
           _o_blade.gameObject.SetActive(false);
            _o_handle.gameObject.SetActive(false);
            _o_glow.gameObject.SetActive(false);
            _o_particles.gameObject.SetActive(false);
            _o_materialization.gameObject.SetActive(false);
         
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
                    audioProcessor.PlaySequencedSound("Frame", 0);
                    _o_handle.gameObject.SetActive(true);
                    _hands.rightHand.SetColorSmooth(HandShaderProps.MainColor, new Color(0.1f, 0, 0.2f, 0.55f), 3);
                    _hands.leftHand.SetColorSmooth(HandShaderProps.MainColor, new Color(0.1f, 0, 0.2f, 0.55f), 3);
                    _hands.rightHand.SetColorSmooth(HandShaderProps.EdgeColor, new Color(0.53f, 0, 0.8f, 0), 3);
                    _hands.leftHand.SetColorSmooth(HandShaderProps.EdgeColor, new Color(0.53f, 0, 0.8f, 0), 3);
                    _o_handle.UpdateDisolveValue(0f);
                    break;
                case 1:
                    audioProcessor.PlaySequencedSound("Frame", 1);
                    _hands.rightHand.SetColorSmooth(HandShaderProps.EdgeColor, new Color(0.53f, 0, 0.8f), 3);
                    _hands.leftHand.SetColorSmooth(HandShaderProps.EdgeColor, new Color(0.53f, 0, 0.8f), 3);
                    _o_handle.UpdateDisolveValue(1f);
                    break;
                
                case 2:
                    audioProcessor.PlaySequencedSound("Frame", 2);
                   _o_blade.gameObject.SetActive(true);
                   _o_blade.UpdateDisolveValue(0f);
                    break;
                 
                case 3:
                    _o_blade.UpdateDisolveValue(1f);
                    _o_materialization.gameObject.SetActive(true);
                    break;
                
                case 4:
                    _o_glow.UpdateDisolveValue(0f);
                    _o_glow.gameObject.SetActive(true);
                    break;
                
                case 5:
                    _o_glow.UpdateDisolveValue(1f);
                    _o_particles.SetActive(true);
                    break;
            }
        }

        protected override void OnHitStartHold()
        {

            throw new System.NotImplementedException();
        }

        protected override void OnHitHolding()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnHitCalled()
        { 
            audioProcessor.PlaySound("Swing");
        }
        protected override void OnHitImpact(string affected)
        {
            base.OnHitImpact(affected);
            switch (affected)
            {
                case "Player":
                   audioProcessor.PlaySound("HitPlayer");
                    break;
                case "Map":
                  audioProcessor.PlaySound("HitMap");
                    break;
            }
        }
        public override void AbilityCalled()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnAbilityReleased()
        {
          //  audioProcessor.PlaySound("Break");
        }
    }


