using System;
using Scripts.Gestures;
using Scripts.PlayerLogic;
using UnityEngine;
public abstract class GrabSystem : MonoBehaviour
{
    public event Action OnGrabStart;
    public event Action OnGrabEnd;
    
    [SerializeField] protected Transform _grabObject;
    
    protected PlayerData _playerData => PlayerData.local;
    
    [Header("Grab Gestures")] 
    [SerializeField] protected string rightHandGrabGesture;
    [SerializeField] protected string leftHandGrabGesture;

    [Header("Movement")] 
    [SerializeField] protected float moveLerpSpeed;
    [SerializeField] protected float rotationSlerpSpeed;
    
    [Header("Main Grab Point")] 
    [SerializeField] protected GrabPoint _mainGrabPoint;
    protected bool _mainGrabbed;
    protected bool _mainGrabReversed;
    protected float _mainGrabPosOffset;
    protected Transform _mainGrabberTransform;
    protected string _mainGrabGesture;
    
    private RecognitionProperties _recognitionProperties = new RecognitionProperties
    {
        positionQuality = 0,
        rotationQuality = 0.9f,
        rootRotationQuality = 0,
    };

    protected abstract void HandleGrab();
    protected abstract void SetGrabObjectTransform();
    
    public void Start()
    {
    //    _playerData = PlayerData.local;
    }
    
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
        HandleGrab();
        SetGrabObjectTransform();
    }
    
    private bool IsHandInGrabZone(Transform hand, GrabPoint grabPoint, ref float grabPos)
    {
        // Calculate the direction vector from the hand to the capture point
        Vector3 captureDirection = grabPoint.GrabPointTransform.position - hand.position;

        // Calculate the distance from the hand to the line on which the capture point lies
        float distanceToLine = Vector3.Cross(captureDirection, grabPoint.GrabPointTransform.right).magnitude;

        // Check if the distance to the line is less than the capture area radius
        if (distanceToLine < grabPoint.GrabPointRadius)
        {
            // Calculate the closest point on the line to the capture point
            Vector3 closestPointOnLine  = grabPoint.GrabPointTransform.position - grabPoint.GrabPointTransform.right * Vector3.Dot(captureDirection, grabPoint.GrabPointTransform.right);

            // Calculate the distance from the closest point on the line to the capture point
            float distanceToClosestPoint = Vector3.Distance(grabPoint.GrabPointTransform.position, closestPointOnLine);

            // Check if the distance to the closest point on the line is less than the capture area length
            if (distanceToClosestPoint < grabPoint.GrabPointLength)
            {
                grabPos = (grabPoint.GrabPointTransform.position - closestPointOnLine).magnitude;
                if (Vector3.Dot(captureDirection, grabPoint.GrabPointTransform.right) < 0)
                    grabPos = -grabPos;
                return true;
            }
        }
        
        return false;
    }
    
    protected bool CheckHandGrab(Transform hand, GrabPoint grabPoint, string gesture, ref Transform currentGrabberTransform, ref string grabGesture, ref bool grabReversed, ref float grabPosOffset)
    {
        if (IsHandInGrabZone(hand, grabPoint, ref grabPosOffset) && RecognizeFrame(gesture))
        {
            currentGrabberTransform = hand;
            grabGesture = gesture;
            grabReversed = Vector3.Dot(hand.right, grabPoint.GrabPointTransform.right) > 0;
            
            return true;
        }

        return false;
    }
    
    protected void SetGrabObjectTransformOneHanded(Transform grabberTransform, GrabPoint grabPoint, float grabPosOffset, bool grabReversed)
    {
        var grabObjectTransform = _grabObject.transform;
        var grabObjectPos = grabObjectTransform.position;

        var localRotation = grabReversed ? Quaternion.Inverse(grabPoint.GrabPointTransform.localRotation) : grabPoint.GrabPointTransform.localRotation;
        grabObjectTransform.rotation = Quaternion.Slerp(grabObjectTransform.rotation, grabberTransform.rotation * localRotation, rotationSlerpSpeed * Time.deltaTime);
        grabObjectPos = Vector3.Lerp(grabObjectPos,grabberTransform.position + ((grabReversed ? grabberTransform.right : -grabberTransform.right) * grabPosOffset) +
                                         (grabObjectPos - grabPoint.GrabPointTransform.position), moveLerpSpeed * Time.deltaTime);
        grabObjectTransform.position = grabObjectPos;
    }
    
    protected bool RecognizeFrame(string grabGesture)
    {
        return Recognizer.RecognizeFrame(_recognitionProperties,
            _playerData.library.supportiveGestures[grabGesture]);
    }
}

[Serializable]
public class GrabPoint
{
    public Transform GrabPointTransform;
    public float GrabPointRadius;
    public float GrabPointLength;
}
