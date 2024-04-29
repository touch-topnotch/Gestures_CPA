using System;
using Scripts.Gestures;
using Scripts.PlayerLogic;
using UnityEngine;
public abstract class GrabSystem : MonoBehaviour
{
    public event Action OnGrabStart;
    public event Action OnGrabEnd;
    
    [SerializeField] protected Transform _grabObject;
    
    protected PlayerData _playerData;
    
    [Header("Grab Gestures")] 
    [SerializeField] protected string rightHandGrabGesture;
    [SerializeField] protected string leftHandGrabGesture;

    [Header("Movement")] 
    [SerializeField] protected float moveLerpSpeed;
    [SerializeField] protected float rotationSlerpSpeed;
    
    [Header("Main Grab Point")] 
    [SerializeField] protected GrabPoint _mainGrabPoint;
    protected bool _mainGrabbed;
    protected float _mainGrabPosOffset;
    protected Transform _mainGrabberTransform;
    protected string _mainGrabGesture;

    protected abstract void HandleGrab();
    protected abstract void SetGrabObjectTransform();
    
    public void SetPlayerData(PlayerData data)
    {
        _playerData = data;
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
    
    private bool IsHandInGrabZone(Transform hand, GrabPoint grabPoint, out float grabPos)
    {
        grabPos = 0f;
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
    
    protected bool CheckHandGrab(Transform hand, GrabPoint grabPoint, string gesture, ref Transform currentGrabberTransform, ref string grabGesture, out float grabPosOffset)
    {
        if (IsHandInGrabZone(hand, grabPoint, out grabPosOffset) && RecognizeFrame(gesture))
        {
            currentGrabberTransform = hand;
            grabGesture = gesture;
            
            return true;
        }

        return false;
    }
    
    protected void SetGrabObjectTransformOneHanded(Transform grabberTransform, GrabPoint grabPoint, float grabPosOffset)
    {
        var grabObjectTransform = _grabObject.transform;
        var grabObjectPos = grabObjectTransform.position;

        grabObjectTransform.rotation = Quaternion.Slerp(grabObjectTransform.rotation, grabberTransform.rotation * grabPoint.GrabPointTransform.localRotation, rotationSlerpSpeed * Time.deltaTime);
        grabObjectPos = Vector3.Lerp(grabObjectPos,grabberTransform.position + (-grabberTransform.right * grabPosOffset) +
                                         (grabObjectPos - grabPoint.GrabPointTransform.position), moveLerpSpeed * Time.deltaTime);
        grabObjectTransform.position = grabObjectPos;
    }
    
    protected bool RecognizeFrame(string grabGesture)
    {
        return _playerData.recognizer.RecognizeFrame(new RecognitionProperties
            {
                positionQuality = 0,
                rotationQuality = 0.9f,
                rootRotationQuality = 0,
            },
            _playerData.library.supportiveGestures[grabGesture], false);
    }

    
}

[Serializable]
public class GrabPoint
{
    public Transform GrabPointTransform;
    public float GrabPointRadius;
    public float GrabPointLength;
}
