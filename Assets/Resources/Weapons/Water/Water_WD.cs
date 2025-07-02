using System;
using System.Collections;
using System.Collections.Generic;
using Components;
using DG.Tweening;
using Scripts.Gestures;
using UnityEngine;

namespace Scripts
{
    
    public class Water_WD : WeaponDesign
    {
        [SerializeField] private GameObject _waterHead;
        [SerializeField] private float _speed;
        [SerializeField] private float _waterSplineAppearSpeed;

        [Header("Visual")]
        [SerializeField] private GameObject _waterPuddle;
        [SerializeField] private Vector3 _puddlePosOffset = new Vector3(0,0.05f, 0);
        [SerializeField] private float _puddleDisappearDuration = 1;
        [SerializeField] private SplineBendingControll _waterSpline;
        [SerializeField] private GameObject _waterSplash;
        [SerializeField] private List<ParticleSystem> _waterSplashParticleSystems;
        [SerializeField] private float _waterDisappearDelay = 1;


        private ControlState _controlState = ControlState.Deactivated;

        private enum ControlState
        {
            Deactivated,
            Casting,
            Bending,
        }


        public override void OnReadyToBeCasted()
        {
            throw new NotImplementedException();
        }

        public override void OnCastCancelled()
        {
            throw new NotImplementedException();
        }

        public override void OnGestureCasted()
        {
            throw new NotImplementedException();
        }

        public override void OnActivated()
        {
            throw new NotImplementedException();
        }

        public override void OnHitStarted()
        {
            throw new NotImplementedException();
        }

        public override void OnHitStopped()
        {
            throw new NotImplementedException();
        }

        public override void OnDeactivated()
        {
            throw new NotImplementedException();
        }

        public override void OnAbilityDestroyed()
        {
            throw new NotImplementedException();
        }

        public override void OnFrameRecognized(string frameName)
        {
            var frameId = GestureMapper.IndexOfName(frameName);
            Debug.Log("Design FrameRecognized " + frameId);

            switch (frameId)
            {
                case 0:
                    _waterPuddle.transform.position = playerData.hands.leftHand.grabPoint.position + _puddlePosOffset;
                    _waterSpline.transform.position = _waterPuddle.transform.position;
                    _waterPuddle.SetActive(true);
                    _controlState = ControlState.Casting;
                    break;
                case 2:
                    _waterSpline.gameObject.SetActive(true);
                    _waterSpline.StartWaterBend(Vector3.up, _speed, _waterSplineAppearSpeed, _speed);
                    break;
                case 4:
                    _waterHead.SetActive(true);
                    _controlState = ControlState.Bending;
                    _waterPuddle.transform.DOScale(Vector3.zero, _puddleDisappearDuration).SetEase(Ease.InSine)
                        .OnComplete(() => _waterPuddle.SetActive(false));
                    break;
            }
        }

        private void Update()
        {
            switch (_controlState)
            {
                case ControlState.Deactivated:
                    break;
                case ControlState.Casting:
                    ControlDuringCast();
                    break;
                case ControlState.Bending:
                    WaterBend();
                    break;
            }
        }

        private void ControlDuringCast()
        {
            _waterPuddle.transform.position = Vector3.Lerp(_waterPuddle.transform.position, playerData.hands.leftHand.grabPoint.position + _puddlePosOffset, 16f * Time.deltaTime);
            _waterSpline.transform.position = Vector3.Lerp(_waterSpline.transform.position, _waterPuddle.transform.position, 20f * Time.deltaTime);
        }

        private void WaterBend()
        {
            var hand =  playerData.hands.rightHand;
            _waterSpline.SetDirection(hand.points[3].transform.forward);
            _waterHead.transform.position = _waterSpline.SplineHeadPosition;
        }

        public override void OnImpact(string affected)
        {
            if (affected.Equals("Untagged")) return; // delete later
            Debug.Log("Impact" + affected);
            _controlState = ControlState.Deactivated;
            _waterSpline.StopWaterBend();
            _waterHead.SetActive(false);

            StartCoroutine(HandleWaterSplash());

        }

        private IEnumerator HandleWaterSplash()
        {
            _waterSplash.transform.position = _waterHead.transform.position;
            _waterSplash.transform.rotation *= Quaternion.FromToRotation(-_waterSplash.transform.up, _waterSpline.SplineHeadDirection);
            _waterSplash.SetActive(true);
            yield return new WaitForSeconds(_waterDisappearDelay);
            foreach (var particleSystem in _waterSplashParticleSystems)
            {
                particleSystem.Stop();
            }
            yield return new WaitForSeconds(_waterDisappearDelay);
            _waterSplash.SetActive(false);
        }
    }
}
