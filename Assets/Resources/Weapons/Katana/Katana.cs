using Components;
using Scripts.Design;
using Scripts.Effects;
using Scripts.Gestures;
using Scripts.HandsLogic;
using Scripts.Systems;
using Scripts.Weapons;
using UnityEngine;

    [RequireComponent(typeof(Melee))]
    public class Katana : WeaponDesign
    {
       
        private PlayerHands _hands => playerData.hands;

        private void Start()
        {
            vfxProcessor.DisableAllObjects();
        }

        private void DissolveItem(GameObject o, int range = 0)
        {
            if (o.TryGetComponent<DissolveSlider>(out var slider))
            {
                slider.UpdateDisolveValue(range);
            }
        }
        public override void OnFrameRecognized(string frameName)
        {
            var frameId = GestureMapper.IndexOfName(frameName);
            
            audioProcessor.ActivateSequencedResource("Frame", frameId);
            
            // на каждом кадре включаем свой вфкс (curVFX.Value = 0)
            vfxProcessor.ActivateSequencedResource("Frame", frameId, (GameObject o)=>
            {
                o.SetActive(true);
                DissolveItem(o, 0);
            });
            
            // на каждом следующем меняем значение для анимации (curVFX.value = 1)
            vfxProcessor.ActivateSequencedResource("Frame", frameId+1, (GameObject o)=>
            {
                DissolveItem(o, 1);
            });
            
            switch (frameId)
            {
                case 0:
                    _hands.rightHand.ChangeColorSmooth( new Color(0.1f, 0, 0.2f, 0.55f), new ColorParams(HandShaderProps.MainColor,3, false));
                    _hands.leftHand.ChangeColorSmooth(new Color(0.1f, 0, 0.2f, 0.55f),new ColorParams(HandShaderProps.MainColor,3, false));
                    _hands.rightHand.ChangeColorSmooth(new Color(0.53f, 0, 0.8f, 0), new ColorParams(HandShaderProps.EdgeColor,3, false));
                    _hands.leftHand.ChangeColorSmooth(new Color(0.53f, 0, 0.8f, 0), new ColorParams(HandShaderProps.EdgeColor,3, false));
                    break;
                case 1:
                    _hands.rightHand.ChangeColorSmooth(new Color(0.53f, 0, 0.8f), new ColorParams(HandShaderProps.EdgeColor,3, false));
                    _hands.leftHand.ChangeColorSmooth( new Color(0.53f, 0, 0.8f), new ColorParams(HandShaderProps.EdgeColor,3, false));
                    break;
                case 2:
                    _hands.leftHand.points[0].position += new Vector3(1, 1, 1);
                    break;
            }
        }

        public override void OnGestureDetected()
        {
            //throw new System.NotImplementedException();
        }

        public override void OnHit()
        { 
            audioProcessor.ActivateResource("Swing");
        }
        public override void OnImpact(string affected)
        {
            switch (affected)
            {
                case "Player":
                   audioProcessor.ActivateResource("HitPlayer");
                    break;
                case "Map":
                  audioProcessor.ActivateResource("HitMap");
                    break;
            }
        }

        public override void OnAbilityReleased()
        {
        }

        public override void OnHitHolds()
        {
            
        }
    }



