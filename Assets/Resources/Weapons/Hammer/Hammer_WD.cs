using System.Collections;
using Components;
using DG.Tweening;
using Scripts.Components;
using Scripts.Gestures;
using UnityEngine;
using UnityEngine.VFX;

namespace Scripts.Resources.Weapons.Hammer
{
    public class Hammer_WD : WeaponDesign
    {
        [SerializeField] private LayerMask _floorMask;

        [Header("Hammer Settings")] [SerializeField] [Range(0, 1f)]
        private float _hammerYSpeed;
         [Header(("SFX Objects"))] [SerializeField]
        private AudioSource _audioSource;
        [SerializeField]
        private AudioClip _pushOne;
        [SerializeField] private AudioClip _pushTwo;
        [SerializeField] private AudioClip _treshina;
        [SerializeField] private AudioClip _fall;
        [Header("VFX Objects")] 
        [SerializeField]
        private VisualEffect _treshinaVFX;
        [SerializeField]
        private VisualEffect _rockVFX;
        [SerializeField] 
        private VisualEffect _materializationController;

        [SerializeField] private Transform hammer;
        [SerializeField] private Transform vfxRoot;
        [SerializeField] private Transform hammerInStoneAnchor;
      
        private int currentState;
        public override void OnFrameRecognized(string frameName)
        {
            
            var frameId = GestureMapper.IndexOfName(frameName);
            Debug.Log("Design FrameRecognized " + frameId);
    
            currentState = frameId;
            switch (frameId)
            {
                case 0:
                    vfxRoot.position = new Vector3(playerData.recognizerCenter.position.x, playerData.anchors.Root.position.y, playerData.recognizerCenter.position.z);
                    _audioSource.PlayOneShot(_pushOne, 0.5f);
                    _treshinaVFX.gameObject.SetActive(true);
                    _treshinaVFX.DOPlayForward();
                    break;
                case 1:
                    _audioSource.PlayOneShot(_pushTwo);
                    break;
                case 2:
                    _audioSource.PlayOneShot(_pushOne, 0.5f);
                    break;
                case 3:
                    _audioSource.PlayOneShot(_pushTwo, 0.3f);
                    _audioSource.PlayOneShot(_treshina, 1);
                    break;
                case 6:
                    _rockVFX.transform.position = vfxRoot.position;
                    _rockVFX.gameObject.SetActive(true);
                    _audioSource.PlayOneShot(_fall);
                    _rockVFX.DOPlayForward();
                    break;
                case 7:
                    hammer.gameObject.SetActive(true);
                    hammer.position = hammerInStoneAnchor.position;
                    _audioSource.PlayOneShot(_treshina);
                    _materializationController.gameObject.SetActive(true);
                    _materializationController.Play();
                    break;

            }
        }

        
        public override void Update()
        {
            if (currentState >=  6 && !gestureRealyCasted)
            {
                var midPoint = ((playerData.hands.leftHand.points[0].position +
                                 playerData.hands.rightHand.points[1].position) / 2);
                var position = hammer.position;
                position = Vector3.Lerp(position, new Vector3(position.x, midPoint.y, position.z), _hammerYSpeed * Time.deltaTime);
                hammer.position = position;
                if(hammer.position.y - playerData.anchors.Body.position.y > 1.65f) 
                {
                    gestureRealyCasted = true;
                    OnGestureRealyCasted();
                }
            }
        }

        public void OnGestureRealyCasted()
        {
            Debug.Log("OnGestureRealyCasted");
            _materializationController.gameObject.SetActive(false);
            _treshinaVFX.transform.DOScale(Vector3.zero, 0.5f);
            _rockVFX.transform.DOScale(Vector3.zero, 0.5f);
        }
        
        public override void OnReadyToBeCasted()
        {

            _rockVFX.transform.localScale = Vector3.one;
            _treshinaVFX.transform.localScale = Vector3.one;
            _rockVFX.gameObject.SetActive(false);
            _treshinaVFX.gameObject.SetActive(false);
            _materializationController.gameObject.SetActive(false);
            hammer.gameObject.SetActive(false);
            _audioSource.clip = null;
            _audioSource.volume = 1;
        }

        public void ActivateGrabSystem()
        {
            
        }
        public override void OnCastCancelled()
        {
            
        }

        private bool gestureRealyCasted = false;
        public override void OnGestureCasted()
        {
            
        }

        public override void OnActivated()
        {
         //   throw new System.NotImplementedException();
        }

        public override void OnHitStarted()
        {
        //    throw new System.NotImplementedException();
        }

        public override void OnHitStopped()
        {
        }

        public override void OnDeactivated()
        {
        }

        public override void OnAbilityDestroyed()
        {
        }

        public override void OnImpact(string affected)
        {
        }
    }
}