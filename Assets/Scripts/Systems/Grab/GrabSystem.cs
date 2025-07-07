using System;
using System.Collections;
using Scripts.Components;
using Scripts.Gestures;
using Scripts.Players;
using Scripts.Static.Definitions;
using Scripts.Weapons;
using Unity.Netcode;
using UnityEngine;


// grab system
// grab handling = распознан ли кулак (это мы проверяем на клиенте) -> OnGrabbed -> ActivatedEvent.Invoke();
// пока клиент не сообщил о OnUngrabbed
// у нас всегда сетится позиция.

namespace Scripts.Systems.Grab
{
    public abstract class GrabSystem : WeaponObserver
    {
        public event Action OnGrabStart;
        public event Action OnGrabEnd;
        public event Action OnSwing;

        [SerializeField] protected Transform _grabObject;

        protected PlayerData _playerData;

        protected Grabber rightHandGrabber;
        protected Grabber leftHandGrabber;

        [Header("Grab Gestures")] [SerializeField]
        protected string rightHandGrabGesture;

        [SerializeField] protected string leftHandGrabGesture;
        [SerializeField] protected float recognizeFailDelay;

        [Header("Movement")] [SerializeField] protected float moveLerpSpeed;
        [SerializeField] protected float rotationSlerpSpeed;

        [Header("Main Grab Point")] [SerializeField]
        protected GrabPoint _mainGrabPoint;

        private WaitForSeconds _waitForRecognizeFail;

        private bool isActivated;
        private bool isOwner;
        
        private RecognitionProperties _recognitionProperties = new RecognitionProperties
        {
            positionQuality = 0,
            rotationQuality = 0.9f,
            rootRotationQuality = 0,
        };

        protected abstract void HandleGrab();
        protected abstract void SetGrabObjectTransform();

        protected virtual void OnGrabStarted()
        {
            OnGrabStart?.Invoke();
        }

        protected virtual void OnGrabEnded()
        {
            OnGrabEnd?.Invoke();
        }
        private void Update()
        {
            if (_playerData is { isOwner: true })
            {
                HandleGrab();
            }
            if(isActivated)
                SetGrabObjectTransform();
          
        }

        private bool IsHandInGrabZone(Transform hand, GrabPoint grabPoint)
        {
            // Calculate the direction vector from the hand to the capture point
            Vector3 captureDirection = grabPoint.GrabPointTransform.position - hand.position;

            // Calculate the distance from the hand to the line on which the capture point lies
            float distanceToLine = Vector3.Cross(captureDirection, grabPoint.GrabPointTransform.right).magnitude;

            // Check if the distance to the line is less than the capture area radius
            var grabPointGrabPointRadius = grabPoint.IsGrabbed ? grabPoint.GrabPointRadius * 2 : grabPoint.GrabPointRadius;
            if (distanceToLine < grabPointGrabPointRadius)
            {
                // Calculate the closest point on the line to the capture point
                Vector3 closestPointOnLine = grabPoint.GrabPointTransform.position - grabPoint.GrabPointTransform.right *
                    Vector3.Dot(captureDirection, grabPoint.GrabPointTransform.right);

                // Calculate the distance from the closest point on the line to the capture point
                float distanceToClosestPoint = Vector3.Distance(grabPoint.GrabPointTransform.position, closestPointOnLine);

                // Check if the distance to the closest point on the line is less than the capture area length
                var grabPointGrabPointLength =
                    grabPoint.IsGrabbed ? grabPoint.GrabPointLength * 1.5f : grabPoint.GrabPointLength;
                if (distanceToClosestPoint < grabPointGrabPointLength)
                {
                    grabPoint.GrabPosOffset = (grabPoint.GrabPointTransform.position - closestPointOnLine).magnitude;
                    if (Vector3.Dot(captureDirection, grabPoint.GrabPointTransform.right) < 0)
                        grabPoint.GrabPosOffset = -grabPoint.GrabPosOffset;
                    return true;
                }
            }

            return false;
        }

        protected bool CheckHandGrab(Grabber grabber, GrabPoint grabPoint, string gesture)
        {
      
            if (grabber.IsGrabbing) return false;
            if (IsHandInGrabZone(grabber.Transform, grabPoint) && RecognizeFrame(gesture))
            {
                grabPoint.Grabber = grabber;
                grabPoint.GrabGesture = gesture;
                grabPoint.GrabReversed = Vector3.Dot(grabber.Transform.right, grabPoint.GrabPointTransform.right) > 0;
                return true;
            }

            return false;
        }

        protected void SetGrabObjectTransformOneHanded(GrabPoint grabPoint)
        {
            var grabObjectTransform = _grabObject.transform;
            var grabObjectPos = grabObjectTransform.position;

            var localRotation = grabPoint.GrabReversed
                ? Quaternion.Inverse(grabPoint.GrabPointTransform.localRotation)
                : grabPoint.GrabPointTransform.localRotation;
            grabObjectTransform.rotation = Quaternion.Slerp(grabObjectTransform.rotation,
                grabPoint.Grabber.Transform.rotation * localRotation, rotationSlerpSpeed * Time.deltaTime);
            Debug.Log(grabPoint + " " +grabObjectPos.ToString());
            grabObjectPos = Vector3.Lerp(grabObjectPos, grabPoint.Grabber.Transform.position +
                                                        ((grabPoint.GrabReversed
                                                             ? grabPoint.Grabber.Transform.right
                                                             : -grabPoint.Grabber.Transform.right) *
                                                         grabPoint.GrabPosOffset) +
                                                        (grabObjectPos - grabPoint.GrabPointTransform.position),
                moveLerpSpeed * Time.deltaTime);
            grabObjectTransform.position = grabObjectPos;
        }

        protected bool RecognizeFrame(string grabGesture)
        {
            return Recognizer.RecognizeFrame(_recognitionProperties, 
                _playerData.gesturesLibrary.supportiveGestures[grabGesture], _playerData.hands, true);
        }

        protected IEnumerator UnGrab(GrabPoint grabPoint, bool needOnGrabEndedRaise)
        {
            grabPoint.IsUnGrabbing = true;
            yield return _waitForRecognizeFail;
            if (needOnGrabEndedRaise) OnGrabEnded();
            grabPoint.IsGrabbed = false;
            grabPoint.IsUnGrabbing = false;
            grabPoint.Grabber.IsGrabbing = false;
        }

        public override void Initialize(PlayerData playerData)
        {
            _playerData = playerData;
            isOwner =  _playerData.isOwner;
            rightHandGrabber = new Grabber(_playerData.hands.rightHand.grabPoint);
            leftHandGrabber = new Grabber(_playerData.hands.leftHand.grabPoint);
            _waitForRecognizeFail = new WaitForSeconds(recognizeFailDelay);
        }
        
        
        public override void OnActivated()
        {
            isActivated = true;
        }

        public override void OnDeactivated()
        {
            isActivated = false;
        }

        public override void OnHitStarted()
        {
            
        }

        public override void OnHitStopped()
        {
        }
   


        public override void OnImpact(Affected affected)
        {
            
        }

        public override void OnReleased()
        {
           isActivated = false;
        }
    }

    [Serializable]
    public class GrabPoint
    {
        public Transform GrabPointTransform;
        public float GrabPointRadius;
        public float GrabPointLength;

        [NonSerialized] public bool IsGrabbed;
        [NonSerialized] public bool GrabReversed;
        [NonSerialized] public float GrabPosOffset;
        [NonSerialized] public Grabber Grabber;
        [NonSerialized] public string GrabGesture;
        [NonSerialized] public Coroutine UnGrabCoroutine;
        [NonSerialized] public bool IsUnGrabbing;
    }

    public class Grabber
    {
        public Transform Transform;
        public bool IsGrabbing;

        public Grabber(Transform transform)
        {
            Transform = transform;
        }
    }
}