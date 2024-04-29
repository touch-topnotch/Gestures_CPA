using System;
using UnityEngine;

public class GrabSystemTwoHanded : GrabSystem
{
    [Header("Secondary Grab Point")] 
    [SerializeField] private GrabPoint _secondaryGrabPoint;
    private bool _secondaryGrabbed;
    private float _secondaryGrabPosOffset;
    private Transform _secondaryGrabberTransform;
    protected string _secondaryGrabGesture;

    [Header("Secondary Grab Boundaries")] 
    [SerializeField] private float grabberTwistAngle;
    [SerializeField] private float angleBetweenGrabbers;

    protected override void HandleGrab()
    {
        if (!_mainGrabbed)
        {
            if (CheckHandGrab(_playerData.hands.rightHand.grabPoint, _mainGrabPoint, rightHandGrabGesture, ref _mainGrabberTransform, ref _mainGrabGesture, out _mainGrabPosOffset) ||
                CheckHandGrab(_playerData.hands.leftHand.grabPoint, _mainGrabPoint, leftHandGrabGesture, ref _mainGrabberTransform, ref _mainGrabGesture, out _mainGrabPosOffset))
            {
                if (!_mainGrabbed && !_secondaryGrabbed) OnGrabStarted();
                _mainGrabbed = true;
            }
        }
        else
        {
            if (!RecognizeFrame(_mainGrabGesture))
            {
                if (_mainGrabbed && !_secondaryGrabbed) OnGrabEnded();
                _mainGrabbed = false;
            }
        }

        if (!_secondaryGrabbed)
        {
            if ((CheckHandGrab(_playerData.hands.rightHand.grabPoint, _secondaryGrabPoint, rightHandGrabGesture, ref _secondaryGrabberTransform, ref _secondaryGrabGesture, out _secondaryGrabPosOffset) ||
                 CheckHandGrab(_playerData.hands.leftHand.grabPoint, _secondaryGrabPoint, leftHandGrabGesture, ref _secondaryGrabberTransform, ref _secondaryGrabGesture, out _secondaryGrabPosOffset)) 
                && IsSecondGrabValid())
            {
                if (!_mainGrabbed && !_secondaryGrabbed) OnGrabStarted();
                _secondaryGrabbed = true;
            }
        }
        else
        {
            if (!RecognizeFrame(_secondaryGrabGesture) || !IsSecondGrabValid())
            {
                if (_secondaryGrabbed && !_mainGrabbed) OnGrabEnded();
                _secondaryGrabbed = false;
            }
        }
    }

    protected override void SetGrabObjectTransform()
    {
        if (_mainGrabbed && !_secondaryGrabbed)
        {
            SetGrabObjectTransformOneHanded(_mainGrabberTransform, _mainGrabPoint, _mainGrabPosOffset);
        }
        if (!_mainGrabbed && _secondaryGrabbed)
        {
            SetGrabObjectTransformOneHanded(_secondaryGrabberTransform, _secondaryGrabPoint, _secondaryGrabPosOffset);
        }
        
        if (_mainGrabbed && _secondaryGrabbed)
        {
            SetGrabObjectTransformTwoHanded();
        }
    }

    private void SetGrabObjectTransformTwoHanded()
    {
        var grabObjectTransform = _grabObject.transform;
        var grabObjectPos = grabObjectTransform.position;

        grabObjectPos = Vector3.Lerp(grabObjectPos, 
            _mainGrabberTransform.position + (_secondaryGrabberTransform.position - _mainGrabberTransform.position).normalized * _mainGrabPosOffset +
            (grabObjectPos - _mainGrabPoint.GrabPointTransform.position), moveLerpSpeed * Time.deltaTime);
        
        grabObjectTransform.position = grabObjectPos;

        Vector3 direction = _secondaryGrabberTransform.position - _mainGrabberTransform.position;
        
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        
        //targetRotation.eulerAngles += new Vector3(0, 0, -_mainGrabberTransform.eulerAngles.x);
        targetRotation *= Quaternion.Euler(0, 0, -_mainGrabberTransform.eulerAngles.x);
        
        grabObjectTransform.rotation = Quaternion.Slerp(grabObjectTransform.rotation, targetRotation, rotationSlerpSpeed * Time.deltaTime);
    }

    private bool IsSecondGrabValid()
    {
        return !_mainGrabbed || (Vector3.Angle(_mainGrabberTransform.right, _secondaryGrabberTransform.right) < grabberTwistAngle
               && Vector3.Angle(-_mainGrabberTransform.right, _secondaryGrabberTransform.position - _mainGrabberTransform.position) < angleBetweenGrabbers);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_mainGrabPoint.GrabPointTransform.position, _mainGrabPoint.GrabPointRadius);
        Gizmos.DrawRay(_mainGrabPoint.GrabPointTransform.position, _mainGrabPoint.GrabPointTransform.right);
        
        Gizmos.DrawWireSphere(_secondaryGrabPoint.GrabPointTransform.position, _secondaryGrabPoint.GrabPointRadius);
        Gizmos.DrawRay(_secondaryGrabPoint.GrabPointTransform.position, _secondaryGrabPoint.GrabPointTransform.right);
    }
}
