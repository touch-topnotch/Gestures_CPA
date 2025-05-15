using System;
using Scripts.Gestures;
using Scripts.PlayerLogic;
using UnityEngine;


public class GrabSystemOneHanded : GrabSystem
{
    protected override void HandleGrab()
    {
        if (!_mainGrabPoint.IsGrabbed)
        {
            if (CheckHandGrab(_playerData.hands.rightHand.grabPoint, _mainGrabPoint, rightHandGrabGesture) ||
                CheckHandGrab(_playerData.hands.leftHand.grabPoint, _mainGrabPoint, leftHandGrabGesture))
            {
                if (!_mainGrabPoint.IsGrabbed) OnGrabStarted();
                if (_mainGrabPoint.UnGrabCoroutine != null) StopCoroutine(_mainGrabPoint.UnGrabCoroutine);
                _mainGrabPoint.IsGrabbed = true;
            }
        }
        else
        {
            if (!RecognizeFrame(_mainGrabPoint.GrabGesture))
            {
                if (!_mainGrabPoint.IsUnGrabbing && _mainGrabPoint.IsGrabbed)
                    _mainGrabPoint.UnGrabCoroutine = StartCoroutine(UnGrab(_mainGrabPoint, _mainGrabPoint.IsGrabbed));
            }
        }
    }

    protected override void SetGrabObjectTransform()
    {
        if (_mainGrabPoint.IsGrabbed)
        {
            SetGrabObjectTransformOneHanded(_mainGrabPoint);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_mainGrabPoint.GrabPointTransform.position, _mainGrabPoint.GrabPointRadius);
        Gizmos.DrawRay(_mainGrabPoint.GrabPointTransform.position, _mainGrabPoint.GrabPointTransform.right);
    }

}