using System;
using UnityEngine;

public class GrabSystemTwoHanded : GrabSystem
{
    [Header("Secondary Grab Point")] [SerializeField]
    private GrabPoint _secondaryGrabPoint;
    
    [Header("Secondary Grab Boundaries")] [SerializeField]
    private float grabberTwistAngle;

    [SerializeField] private float angleBetweenGrabbers;

    protected override void HandleGrab()
    {
        if (!_mainGrabPoint.IsGrabbed)
        {
            if (CheckHandGrab(_playerData.hands.rightHand.grabPoint, _mainGrabPoint, rightHandGrabGesture) ||
                CheckHandGrab(_playerData.hands.leftHand.grabPoint, _mainGrabPoint, leftHandGrabGesture))
            {
                if (!_mainGrabPoint.IsGrabbed && !_secondaryGrabPoint.IsGrabbed) OnGrabStarted();
                if (_mainGrabPoint.UnGrabCoroutine != null) StopCoroutine(_mainGrabPoint.UnGrabCoroutine);
                _mainGrabPoint.IsGrabbed = true;
            }
        }
        else
        {
            if (!RecognizeFrame(_mainGrabPoint.GrabGesture))
            {
                if (!_mainGrabPoint.IsUnGrabbing && _mainGrabPoint.IsGrabbed)
                    _mainGrabPoint.UnGrabCoroutine = StartCoroutine(UnGrab(_mainGrabPoint, 
                        _mainGrabPoint.IsGrabbed && !_secondaryGrabPoint.IsGrabbed));
            }
        }

        if (!_secondaryGrabPoint.IsGrabbed)
        {
            if ((CheckHandGrab(_playerData.hands.rightHand.grabPoint, _secondaryGrabPoint, rightHandGrabGesture) ||
                 CheckHandGrab(_playerData.hands.leftHand.grabPoint, _secondaryGrabPoint, leftHandGrabGesture))
                && IsSecondGrabValid())
            {
                if (!_mainGrabPoint.IsGrabbed && !_secondaryGrabPoint.IsGrabbed) OnGrabStarted();
                if (_secondaryGrabPoint.UnGrabCoroutine != null) StopCoroutine(_secondaryGrabPoint.UnGrabCoroutine);
                _secondaryGrabPoint.IsGrabbed = true;
            }
        }
        else
        {
            if (!RecognizeFrame(_secondaryGrabPoint.GrabGesture) || !IsSecondGrabValid())
            {
                if (!_secondaryGrabPoint.IsUnGrabbing && _secondaryGrabPoint.IsGrabbed)
                    _secondaryGrabPoint.UnGrabCoroutine = StartCoroutine(UnGrab(_secondaryGrabPoint,
                    _secondaryGrabPoint.IsGrabbed && !_mainGrabPoint.IsGrabbed));
            }
        }
    }

    protected override void SetGrabObjectTransform()
    {
        if (_mainGrabPoint.IsGrabbed && !_secondaryGrabPoint.IsGrabbed)
        {
            SetGrabObjectTransformOneHanded(_mainGrabPoint);
        }

        if (!_mainGrabPoint.IsGrabbed && _secondaryGrabPoint.IsGrabbed)
        {
            SetGrabObjectTransformOneHanded(_secondaryGrabPoint);
        }

        if (_mainGrabPoint.IsGrabbed && _secondaryGrabPoint.IsGrabbed)
        {
            SetGrabObjectTransformTwoHanded();
        }
    }

    private void SetGrabObjectTransformTwoHanded()
    {
        var grabObjectTransform = _grabObject.transform;
        var grabObjectPos = grabObjectTransform.position;

        grabObjectPos = Vector3.Lerp(grabObjectPos,
            _mainGrabPoint.GrabberTransform.position +
            (_secondaryGrabPoint.GrabberTransform.position - _mainGrabPoint.GrabberTransform.position).normalized * _mainGrabPoint.GrabPosOffset +
            (grabObjectPos - _mainGrabPoint.GrabPointTransform.position), moveLerpSpeed * Time.deltaTime);

        grabObjectTransform.position = grabObjectPos;

        Vector3 direction = _secondaryGrabPoint.GrabberTransform.position - _mainGrabPoint.GrabberTransform.position;

        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        targetRotation *= Quaternion.Euler(0, 0, -_mainGrabPoint.GrabberTransform.eulerAngles.x);

        grabObjectTransform.rotation = Quaternion.Slerp(grabObjectTransform.rotation, targetRotation,
            rotationSlerpSpeed * Time.deltaTime);
    }

    private bool IsSecondGrabValid()
    {
        if (!_mainGrabPoint.IsGrabbed)
            return true;

        var twistAngle = Vector3.Angle(_mainGrabPoint.GrabberTransform.right, _secondaryGrabPoint.GrabberTransform.right);
        var angle = Vector3.Angle(-_mainGrabPoint.GrabberTransform.right,
            _secondaryGrabPoint.GrabberTransform.position - _mainGrabPoint.GrabberTransform.position);

        return ((twistAngle < grabberTwistAngle || 180 - twistAngle < grabberTwistAngle)
            && angle < angleBetweenGrabbers || 180 - angle < angleBetweenGrabbers);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_mainGrabPoint.GrabPointTransform.position, _mainGrabPoint.GrabPointRadius);
        Gizmos.DrawRay(_mainGrabPoint.GrabPointTransform.position, _mainGrabPoint.GrabPointTransform.right);

        Gizmos.DrawWireSphere(_secondaryGrabPoint.GrabPointTransform.position, _secondaryGrabPoint.GrabPointRadius);
        Gizmos.DrawRay(_secondaryGrabPoint.GrabPointTransform.position, _secondaryGrabPoint.GrabPointTransform.right);
    }
}