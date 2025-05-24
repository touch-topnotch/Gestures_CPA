using System;
using System.Collections;
using System.Collections.Generic;
using Components;
using Scripts.Gestures;
using UnityEngine;

namespace Scripts
{
    public class Lightning_WD : WeaponDesign
    {
        [SerializeField] private GameObject _lightningObject;
        public float speed;
        public float rotationSpeed;
        
        [SerializeField] private GameObject _explosionObject;
        [SerializeField] private float _explosionDuration;


        private Coroutine lightningMove;
        private void Awake()
        {
            _lightningObject.SetActive(false);
        }

        public override void OnFrameRecognized(string frameName)
        {
            var frameId = GestureMapper.IndexOfName(frameName);

            switch (frameId)
            {
                case 2:
                    _lightningObject.transform.position =
                        (playerData.hands.rightHand.grabPoint.position + playerData.hands.leftHand.grabPoint.position) / 2f;
                    _lightningObject.SetActive(true);
                    break;
                case 4:
                    lightningMove = StartCoroutine(StartLightningBall());
                    break;
            }
        }
        
        private IEnumerator StartLightningBall()
        {
            var hand = playerData.hands.rightHand;
            var initialIndexDir = hand.points[3].transform.forward;
            var moveDir = initialIndexDir;
            while (true)
            {
                var fingerDir = hand.points[3].transform.forward;
                moveDir = Vector3.Slerp(moveDir, fingerDir, rotationSpeed * Time.deltaTime);
                
                _lightningObject.transform.position += fingerDir * (speed * Time.deltaTime);

                yield return null;
            }
        }
        
        private IEnumerator Explosion()
        {
            _explosionObject.transform.position = _lightningObject.transform.position;
            _explosionObject.SetActive(true);
            yield return new WaitForSeconds(_explosionDuration);
            _explosionObject.SetActive(false);
        }

        public override void OnGestureDetected()
        {
           
        }

        public override void OnHit()
        {
            
        }

        public override void OnImpact(string affected)
        {
           
        }

        public override void OnAbilityReleased()
        {
            _lightningObject.SetActive(false);
            StopCoroutine(lightningMove);
            StartCoroutine(Explosion());
        }

        public override void OnHitHolds()
        {
            
        }
    }
}
