using System;
using Scripts.Gestures;
using Scripts.PlayerLogic;
using UnityEngine;


public class GrabSystemOneHanded : GrabSystem
{
    protected override void HandleGrab()
    {
        if (!_mainGrabbed)
        {
            if (CheckHandGrab(_playerData.hands.rightHand.grabPoint, _mainGrabPoint, rightHandGrabGesture, ref _mainGrabberTransform, ref _mainGrabGesture, ref _mainGrabReversed, ref _mainGrabPosOffset) ||
                CheckHandGrab(_playerData.hands.leftHand.grabPoint, _mainGrabPoint, leftHandGrabGesture, ref _mainGrabberTransform, ref _mainGrabGesture, ref _mainGrabReversed, ref _mainGrabPosOffset))
            {
                if (!_mainGrabbed) OnGrabStarted();
                _mainGrabbed = true;
            }
        }
        else
        {
            if (!RecognizeFrame(_mainGrabGesture))
            {
                if (_mainGrabbed) OnGrabEnded();
                _mainGrabbed = false;
            }
        }
    }
    
    protected override void SetGrabObjectTransform()
    {
        if (_mainGrabbed)
        {
            SetGrabObjectTransformOneHanded(_mainGrabberTransform, _mainGrabPoint, _mainGrabPosOffset, _mainGrabReversed);
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_mainGrabPoint.GrabPointTransform.position, _mainGrabPoint.GrabPointRadius);
        Gizmos.DrawRay(_mainGrabPoint.GrabPointTransform.position, _mainGrabPoint.GrabPointTransform.right);
    }
}

